using System;
using System.Linq;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Shared.Audio.Events;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.GameTicking.Prototypes;
using Content.Shared._Starlight.CCVar;
using Robust.Client;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Audio;

// Part of ContentAudioSystem that is responsible for lobby music playing/stopping and round-end sound-effect.
public sealed partial class ContentAudioSystem
{
    [Dependency] private IBaseClient _client = default!;
    [Dependency] private ClientGameTicker _gameTicker = default!;
    [Dependency] private IResourceCache _resourceCache = default!;

    private readonly AudioParams _lobbySoundtrackParams = new(-5f, 1, 0, 0, 0, false, 0f);
    private readonly AudioParams _roundEndSoundEffectParams = new(-5f, 1, 0, 0, 0, false, 0f);

    /// <summary>
    /// EntityUid of lobby restart sound component.
    /// </summary>
    private EntityUid? _lobbyRoundRestartAudioStream;

    /// <summary>
    /// Shuffled list of soundtrack file-names.
    /// </summary>
    private string[]? _lobbyPlaylist;

    /// <summary>
    /// Short info about lobby soundtrack currently playing. Is null if soundtrack is not playing.
    /// </summary>
    private LobbySoundtrackInfo? _lobbySoundtrackInfo;

    private Action<LobbySoundtrackChangedEvent>? _lobbySoundtrackChanged;

    /// <summary>
    /// Event for subscription on lobby soundtrack changes.
    /// </summary>
    public event Action<LobbySoundtrackChangedEvent>? LobbySoundtrackChanged
    {
        add
        {
            if (value != null)
            {
                if (_lobbySoundtrackInfo != null)
                {
                    value(new LobbySoundtrackChangedEvent(_lobbySoundtrackInfo.Filename));
                }

                _lobbySoundtrackChanged += value;
            }
        }
        remove => _lobbySoundtrackChanged -= value;
    }

    /// <summary>
    /// Initializes subscriptions that are related to lobby music.
    /// </summary>
    private void InitializeLobbyMusic()
    {
        Subs.CVar(_configManager, CCVars.LobbyMusicEnabled, LobbyMusicCVarChanged);
        Subs.CVar(_configManager, CCVars.LobbyMusicVolume, LobbyMusicVolumeCVarChanged);

        _state.OnStateChanged += StateManagerOnStateChanged;

        _client.PlayerLeaveServer += OnLeave;

        SubscribeNetworkEvent<LobbyMusicStopEvent>(OnLobbySongStopped);
        SubscribeNetworkEvent<LobbyPlaylistChangedEvent>(OnLobbySongChanged);
    }

    /// <summary>
    /// Fired when the lobby playlist changes (track membership, shuffle, etc).
    /// </summary>
    public event Action? LobbyPlaylistChanged;

    private void OnLobbySongStopped(LobbyMusicStopEvent ev)
    {
        EndLobbyMusic();
    }

    private void StateManagerOnStateChanged(StateChangedEventArgs args)
    {
        switch (args.NewState)
        {
            case LobbyState:
                StartLobbyMusic();
                break;
            default:
                EndLobbyMusic();
                break;
        }
    }

    private void OnLeave(object? sender, PlayerEventArgs args)
    {
        EndLobbyMusic();
    }

    #region Starlight

    /// <summary>
    /// Whether lobby music is currently paused.
    /// </summary>
    public bool LobbyMusicPaused => _lobbyPauseStarted != null;
    private TimeSpan? _lobbyPauseStarted;
    private float _lobbyPausePosition;

    /// <summary>
    /// Current track length in seconds, if any.
    /// </summary>
    public float? LobbyTrackLengthSeconds { get; private set; }

    /// <summary>
    /// Current playback position in seconds, if any. Never throws (dead audio device safe).
    /// </summary>
    public float? LobbyTrackPositionSeconds
    {
        get
        {
            if (_lobbySoundtrackInfo == null)
                return null;
            try
            {
                if (!TryComp(_lobbySoundtrackInfo.MusicStreamEntityUid, out AudioComponent? comp))
                    return null;
                return comp.PlaybackPosition;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Seek the current lobby track, keeping autoplay timing intact.
    /// </summary>
    public void SeekLobbyTrack(float seconds)
    {
        if (_lobbySoundtrackInfo == null || LobbyTrackLengthSeconds is not { } length)
            return;

        var clamped = Math.Clamp(seconds, 0f, length);
        if (TryComp(_lobbySoundtrackInfo.MusicStreamEntityUid, out AudioComponent? comp))
            comp.PlaybackPosition = clamped;
        if (LobbyMusicPaused)
        {
            // Daiquiri: store even if the stream entity is already evicted from memory.
            _lobbyPausePosition = clamped;
            _lobbyPauseStarted = _timing.CurTime;
        }
        _lobbySoundtrackInfo = _lobbySoundtrackInfo with
        {
            NextTrackOn = _timing.CurTime + TimeSpan.FromSeconds(Math.Max(0.0, length - clamped))
        };
    }

    /// <summary>
    /// Pause or resume the current lobby track, keeping autoplay timing intact.
    /// </summary>
    public void SetLobbyMusicPaused(bool paused)
    {
        if (_lobbySoundtrackInfo == null)
            return;
        if (paused == LobbyMusicPaused)
            return;

        if (paused)
        {
            try
            {
                if (TryComp(_lobbySoundtrackInfo.MusicStreamEntityUid, out AudioComponent? pauseComp))
                {
                    _lobbyPausePosition = pauseComp.PlaybackPosition;
                    pauseComp.Pause();
                }
            }
            catch
            {
                // Dead audio device: keep pause state, stream gets rebuilt on resume.
            }
            _lobbyPauseStarted = _timing.CurTime;
        }
        else
        {
            // Daiquiri: always rebuild the stream on resume and never depend on the old
            // entity: a long-paused source can be evicted from memory (OS pressure,
            // device sleep, driver cleanup) and StartPlaying won't revive it.
            _lobbyPauseStarted = null;
            var file = _lobbySoundtrackInfo.Filename;
            var pos = _lobbyPausePosition;
            try
            {
                EndLobbyMusicSilent();
                PlaySoundtrack(file);
                if (_lobbySoundtrackInfo != null)
                {
                    SeekLobbyTrack(pos);
                    return;
                }
                _sawmill.Warning("Lobby resume failed: stream did not start.");
            }
            catch (Exception e)
            {
                _sawmill.Error($"Lobby resume failed: {e}");
                _lobbySoundtrackInfo = null;
                LobbyTrackLengthSeconds = null;
            }
            // Resume failed: tell the UI the music is stopped so the button state
            // resets and the user can restart via skip/track select.
            _lobbySoundtrackChanged?.Invoke(new LobbySoundtrackChangedEvent());
        }
    }

    /// <summary>
    /// Stops the current stream without firing change events or clearing pause position.
    /// </summary>
    private void EndLobbyMusicSilent()
    {
        if (_lobbySoundtrackInfo == null)
            return;
        _audio.Stop(_lobbySoundtrackInfo.MusicStreamEntityUid);
        _lobbySoundtrackInfo = null;
        LobbyTrackLengthSeconds = null;
    }

    private void OnRoundEndCancelMessage(RoundEndCancelMessageEvent ev) => EndLobbyMusic();

    /// <summary>
    /// Whether there is a lobby playlist available to skip through.
    /// </summary>
    public bool HasLobbyPlaylist => _lobbyPlaylist is { Length: > 0 };

    /// <summary>
    /// Current lobby playlist filenames, LOCAL view: server order minus tracks
    /// this player excluded. Never leaves this client.
    /// </summary>
    public IReadOnlyList<string>? LobbyPlaylistTracks => LocalPool() is { Length: > 0 } pool ? pool : null;

    /// <summary>
    /// Currently playing lobby track filename, if any.
    /// </summary>
    public string? CurrentLobbyTrack => _lobbySoundtrackInfo?.Filename;

    /// <summary>
    /// Daiquiri: per-playlist tracks excluded LOCALLY by this player. Never sent to server.
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _localRemovedTracks = new();

    /// <summary>
    /// Daiquiri: removed pool tracks that belong to no known playlist (stale server pool).
    /// </summary>
    private readonly HashSet<string> _localRemovedGlobal = new();

    private bool _queueStateLoaded;
    private readonly HashSet<string> _touchedPlaylists = new();

    /// <summary>
    /// Daiquiri: persisted local queue order. Null = follow server order.
    /// </summary>
    private List<string>? _localOrder;

    /// <summary>
    /// Daiquiri: load persisted queue state, then seed turnon=false playlists
    /// the player never touched. Runs once, lazily.
    /// </summary>
    private void EnsureQueueState()
    {
        if (_queueStateLoaded)
            return;
        _queueStateLoaded = true;
        LoadQueueState();
        foreach (var playlist in _proto.EnumeratePrototypes<LobbyPlaylistPrototype>())
        {
            if (playlist.TurnOn)
                continue;
            if (_touchedPlaylists.Contains(playlist.ID))
                continue;
            if (_localRemovedTracks.ContainsKey(playlist.ID))
                continue;
            _localRemovedTracks[playlist.ID] =
                new HashSet<string>(playlist.Tracks.Select(t => t.ToString()));
        }
    }

    private void LoadQueueState()
    {
        // Daiquiri: entries are "playlist|track", ";"-separated.
        foreach (var entry in _configManager.GetCVar(StarlightCCVars.LobbyQueueRemoved)
                     .Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = entry.Split('|', 2);
            if (parts.Length != 2)
                continue;
            if (parts[0] == "*")
            {
                _localRemovedGlobal.Add(parts[1]);
                continue;
            }
            if (!_localRemovedTracks.TryGetValue(parts[0], out var set))
            {
                set = new HashSet<string>();
                _localRemovedTracks[parts[0]] = set;
            }
            set.Add(parts[1]);
        }
        foreach (var playlist in _configManager.GetCVar(StarlightCCVars.LobbyQueueTouched)
                     .Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            _touchedPlaylists.Add(playlist);
        }
        var savedOrder = _configManager.GetCVar(StarlightCCVars.LobbyQueueOrder)
            .Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (savedOrder.Count > 0)
            _localOrder = savedOrder;
    }

    private void SaveQueueState()
    {
        var removed = string.Join(';', _localRemovedTracks
            .SelectMany(kv => kv.Value.Select(t => $"{kv.Key}|{t}"))
            .Concat(_localRemovedGlobal.Select(t => $"*|{t}")));
        _configManager.SetCVar(StarlightCCVars.LobbyQueueRemoved, removed);
        _configManager.SetCVar(StarlightCCVars.LobbyQueueTouched, string.Join(';', _touchedPlaylists));
        _configManager.SetCVar(StarlightCCVars.LobbyQueueOrder, string.Join(';', _localOrder ?? new List<string>()));
        // Daiquiri: flush ARCHIVE cvars to disk now — the client is often killed, not closed.
        _configManager.SaveToFile();
    }

    /// <summary>
    /// Add or remove a track from a lobby playlist. Local only: affects just this player's pool.
    /// </summary>
    public void SetTrackMembership(string playlistId, string trackPath, bool add)
    {
        EnsureQueueState();
        if (add)
        {
            if (_localRemovedTracks.TryGetValue(playlistId, out var set))
                set.Remove(trackPath);
        }
        else
        {
            if (!_localRemovedTracks.TryGetValue(playlistId, out var set))
            {
                set = new HashSet<string>();
                _localRemovedTracks[playlistId] = set;
            }
            set.Add(trackPath);
        }
        _touchedPlaylists.Add(playlistId);
        ReconcileOrder();
        PauseIfPoolEmpty();
        LobbyPlaylistChanged?.Invoke();
    }

    /// <summary>
    /// Daiquiri: remove everything currently in the pool. Local only.
    /// </summary>
    public void ClearQueue()
    {
        EnsureQueueState();
        if (_lobbyPlaylist == null)
            return;
        foreach (var track in _lobbyPlaylist)
        {
            var containers = _proto.EnumeratePrototypes<LobbyPlaylistPrototype>()
                .Where(p => p.Tracks.Any(t => t.ToString() == track))
                .Select(p => p.ID)
                .ToList();
            if (containers.Count == 0)
            {
                _localRemovedGlobal.Add(track);
                continue;
            }
            foreach (var id in containers)
            {
                if (!_localRemovedTracks.TryGetValue(id, out var set))
                {
                    set = new HashSet<string>();
                    _localRemovedTracks[id] = set;
                }
                set.Add(track);
                _touchedPlaylists.Add(id);
            }
        }
        SaveQueueState();
        PauseIfPoolEmpty();
        LobbyPlaylistChanged?.Invoke();
    }

    /// <summary>
    /// Daiquiri: rebuild the persisted local order from the current pool state.
    /// Keeps the saved order stable, drops gone tracks, appends fresh ones.
    /// </summary>
    private void ReconcileOrder()
    {
        EnsureQueueState();
        if (_lobbyPlaylist is not { Length: > 0 })
        {
            SaveQueueState();
            return;
        }
        var server = new HashSet<string>(_lobbyPlaylist);
        var order = _localOrder ?? new List<string>();
        var kept = order.Where(t => server.Contains(t) && !IsLocallyExcluded(t)).ToList();
        kept.AddRange(_lobbyPlaylist.Where(t => !kept.Contains(t) && !IsLocallyExcluded(t)));
        _localOrder = kept;
        SaveQueueState();
    }

    /// <summary>
    /// Include or exclude ALL tracks of a playlist. Local only.
    /// </summary>
    public void SetPlaylistAll(string playlistId, bool include)
    {
        EnsureQueueState();
        if (include)
        {
            _localRemovedTracks.Remove(playlistId);
        }
        else
        {
            if (!_proto.TryIndex<LobbyPlaylistPrototype>(playlistId, out var proto))
                return;
            _localRemovedTracks[playlistId] =
                new HashSet<string>(proto.Tracks.Select(t => t.ToString()));
        }
        _touchedPlaylists.Add(playlistId);
        ReconcileOrder();
        PauseIfPoolEmpty();
        LobbyPlaylistChanged?.Invoke();
    }

    /// <summary>
    /// Whether a track counts as included in a playlist for this player.
    /// </summary>
    public bool IsTrackIncluded(string playlistId, string trackPath)
    {
        EnsureQueueState();
        if (_localRemovedTracks.TryGetValue(playlistId, out var removed) && removed.Contains(trackPath))
            return false;
        return _proto.TryIndex<LobbyPlaylistPrototype>(playlistId, out var proto)
            && proto.Tracks.Any(t => t.ToString() == trackPath);
    }

    /// <summary>
    /// Daiquiri: empty pool pauses playback (vinyl/marquee freeze via pause state).
    /// </summary>
    private void PauseIfPoolEmpty()
    {
        if (LocalPool() is { Length: > 0 })
            return;
        if (_lobbySoundtrackInfo != null && !LobbyMusicPaused)
            SetLobbyMusicPaused(true);
    }

    /// <summary>
    /// Server pool minus locally excluded tracks, in the persisted local order.
    /// A track plays while at least one playlist containing it still includes it.
    /// </summary>
    private string[] LocalPool()
    {
        EnsureQueueState();
        if (_lobbyPlaylist is not { Length: > 0 })
            return [];
        if (_localOrder != null)
        {
            var ordered = _localOrder
                .Where(t => _lobbyPlaylist.Contains(t) && !IsLocallyExcluded(t))
                .ToArray();
            if (ordered.Length > 0)
                return ordered;
            // Daiquiri: stale-empty order (e.g. re-added tracks) falls back to live pool.
        }
        if (_localRemovedTracks.Count == 0 && _localRemovedGlobal.Count == 0)
            return _lobbyPlaylist;
        return _lobbyPlaylist.Where(t => !IsLocallyExcluded(t)).ToArray();
    }

    private bool IsLocallyExcluded(string track)
    {
        // Daiquiri: explicitly removed tracks with no containing playlist (stale pool entries).
        if (_localRemovedGlobal.Contains(track))
            return true;
        var containers = _proto.EnumeratePrototypes<LobbyPlaylistPrototype>()
            .Where(p => p.Tracks.Any(t => t.ToString() == track))
            .Select(p => p.ID)
            .ToList();
        if (containers.Count == 0)
            return false;
        return containers.All(id =>
            _localRemovedTracks.TryGetValue(id, out var removed) && removed.Contains(track));
    }

    /// <summary>
    /// Manually switch to a specific lobby track by filename.
    /// </summary>
    public void PlayLobbyTrack(string filename)
    {
        var pool = LocalPool();
        if (pool is not { Length: > 0 }
            || Array.IndexOf(pool, filename) < 0
            || !_configManager.GetCVar(CCVars.LobbyMusicEnabled)
            || _state.CurrentState is not LobbyState)
        {
            return;
        }

        EndLobbyMusic();
        PlaySoundtrack(filename);
    }

    /// <summary>
    /// Play a lobby track even if it is not in the current server pool
    /// (used when a click auto-adds the track: the pool updates async).
    /// </summary>
    public void PlayLobbyTrackForced(string filename)
    {
        if (!_configManager.GetCVar(CCVars.LobbyMusicEnabled)
            || _state.CurrentState is not LobbyState)
        {
            return;
        }

        EndLobbyMusic();
        PlaySoundtrack(filename);
    }

    /// <summary>
    /// Manually switch to the next lobby track. Wraps around at the end.
    /// </summary>
    public void PlayNextLobbyTrack()
    {
        SkipLobbyTrack(1);
    }

    /// <summary>
    /// Manually switch to the previous lobby track. Wraps around at the start.
    /// </summary>
    public void PlayPreviousLobbyTrack()
    {
        SkipLobbyTrack(-1);
    }

    private void SkipLobbyTrack(int direction)
    {
        var pool = LocalPool();
        if (pool is not { Length: > 0 }
            || !_configManager.GetCVar(CCVars.LobbyMusicEnabled)
            || _state.CurrentState is not LobbyState)
        {
            return;
        }

        var current = _lobbySoundtrackInfo?.Filename;
        var index = current == null ? -1 : Array.IndexOf(pool, current);
        var nextIndex = (index + direction + pool.Length) % pool.Length;

        PlayLobbyTrack(pool[nextIndex]);
    }

    #endregion

    private void LobbyMusicVolumeCVarChanged(float volume)
    {
        if (_lobbySoundtrackInfo != null)
        {
            _audio.SetVolume(
                _lobbySoundtrackInfo.MusicStreamEntityUid,
                _lobbySoundtrackParams.Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar(CCVars.LobbyMusicVolume))
            );
        }
    }

    private void LobbyMusicCVarChanged(bool musicEnabled)
    {
        if (musicEnabled && _state.CurrentState is LobbyState)
        {
            StartLobbyMusic();
        }
        else
        {
            EndLobbyMusic();
        }
    }

    private void OnLobbySongChanged(LobbyPlaylistChangedEvent playlistChangedEvent)
    {
        var playlist = playlistChangedEvent.Playlist;
        var current = _lobbySoundtrackInfo?.Filename;

        // Restart only if the current track is no longer in the playlist
        if (current != null
            && _lobbyPlaylist != null
            && _lobbyPlaylist.Contains(current)
            && playlist.Contains(current))
        {
            _lobbyPlaylist = playlist;
            LobbyPlaylistChanged?.Invoke();
            return;
        }

        _lobbyPlaylist = playlist;
        // Daiquiri: fresh server pool — reconcile the persisted local order with it.
        ReconcileOrder();
        EndLobbyMusic();
        StartLobbyMusic(playlist);
        LobbyPlaylistChanged?.Invoke();
    }

    /// <summary>
    /// Re-starts playing lobby music from playlist, last sent from server. if there is currently none - does nothing.
    /// </summary>
    private void StartLobbyMusic()
    {
        if (_lobbyPlaylist == null || _lobbyPlaylist.Length == 0)
        {
            return;
        }

        StartLobbyMusic(_lobbyPlaylist);
    }

    /// <summary>
    /// Starts playing lobby music from playlist. If playlist is empty, or lobby music setting is turned off - does nothing.
    /// </summary>
    /// <param name="playlist">Array of soundtrack filenames for lobby playlist.</param>
    private void StartLobbyMusic(string[] playlist)
    {
        if (_lobbySoundtrackInfo != null || !_configManager.GetCVar(CCVars.LobbyMusicEnabled))
            return;

        _lobbyPlaylist = playlist;
        if (_lobbyPlaylist.Length == 0)
        {
            return;
        }

        PlaySoundtrack(playlist[0]);
    }

    private void PlaySoundtrack(string soundtrackFilename)
    {
        if (!_resourceCache.TryGetResource(new ResPath(soundtrackFilename), out AudioResource? audio))
        {
            return;
        }

        var playResult = _audio.PlayGlobal(
            soundtrackFilename,
            Filter.Local(),
            false,
            _lobbySoundtrackParams.WithVolume(_lobbySoundtrackParams.Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar(CCVars.LobbyMusicVolume)))
        );
        if (playResult == null)
        {
            _sawmill.Warning(
                $"Tried to play lobby soundtrack '{{Filename}}' using {nameof(SharedAudioSystem)}.{nameof(SharedAudioSystem.PlayGlobal)} but it returned default value of EntityUid!",
                soundtrackFilename);
            return;
        }

        var nextTrackOn = _timing.CurTime + audio.AudioStream.Length;
        _lobbySoundtrackInfo = new LobbySoundtrackInfo(soundtrackFilename, nextTrackOn, playResult.Value.Entity);
        _lobbyPauseStarted = null;
        LobbyTrackLengthSeconds = (float) audio.AudioStream.Length.TotalSeconds;

        var lobbySongChangedEvent = new LobbySoundtrackChangedEvent(soundtrackFilename);
        _lobbySoundtrackChanged?.Invoke(lobbySongChangedEvent);
    }

    private void EndLobbyMusic()
    {
        if (_lobbySoundtrackInfo == null)
        {
            return;
        }

        _audio.Stop(_lobbySoundtrackInfo.MusicStreamEntityUid);
        _lobbySoundtrackInfo = null;
        _lobbyPauseStarted = null;
        LobbyTrackLengthSeconds = null;
        var lobbySongChangedEvent = new LobbySoundtrackChangedEvent();
        _lobbySoundtrackChanged?.Invoke(lobbySongChangedEvent);
    }

    private void PlayRestartSound(RoundRestartCleanupEvent ev)
    {
        if (!_configManager.GetCVar(CCVars.RestartSoundsEnabled))
            return;

        var file = _gameTicker.RestartSound;
        if (ResolvedSoundSpecifier.IsNullOrEmpty(file))
        {
            return;
        }

        _lobbyRoundRestartAudioStream = _audio.PlayGlobal(
            file,
            Filter.Local(),
            false,
            _roundEndSoundEffectParams.WithVolume(_roundEndSoundEffectParams.Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar(CCVars.LobbyMusicVolume)))
        )?.Entity;
    }

    private void ShutdownLobbyMusic()
    {
        _state.OnStateChanged -= StateManagerOnStateChanged;

        _client.PlayerLeaveServer -= OnLeave;

        EndLobbyMusic();
    }

    private void UpdateLobbyMusic()
    {
        if (_lobbySoundtrackInfo == null)
        {
            return;
        }

        // Daiquiri: never auto-advance while paused (frozen pause keeps its state).
        if (LobbyMusicPaused)
        {
            return;
        }

        var pool = LocalPool();
        if (pool is not { Length: > 0 })
        {
            // Daiquiri: empty pool — stop only dead/finished streams so the UI falls
            // back; live ones (e.g. forced click-play) keep playing.
            var done = _timing.CurTime >= _lobbySoundtrackInfo.NextTrackOn;
            if (!done
                && (TryComp(_lobbySoundtrackInfo.MusicStreamEntityUid, out AudioComponent? aliveComp) && aliveComp.Playing))
            {
                return;
            }
            EndLobbyMusic();
            return;
        }

        var finished = _timing.CurTime >= _lobbySoundtrackInfo.NextTrackOn;
        if (!finished
            && (!TryComp(_lobbySoundtrackInfo.MusicStreamEntityUid, out AudioComponent? comp) || !comp.Playing))
        {
            // Safety net: stream died or finished early (e.g. bad duration metadata).
            _sawmill.Debug("Lobby track finished early, advancing playlist.");
            finished = true;
        }

        if (!finished)
        {
            return;
        }

        var nextSoundtrackFilename = GetNextSoundtrackFromPlaylist(_lobbySoundtrackInfo.Filename, pool);
        EndLobbyMusic();
        PlaySoundtrack(nextSoundtrackFilename);
    }

    private static string GetNextSoundtrackFromPlaylist(string currentSoundtrackFilename, string[] playlist)
    {
        var indexOfCurrent = Array.IndexOf(playlist, currentSoundtrackFilename);
        var nextTrackIndex = indexOfCurrent + 1;
        if (nextTrackIndex > playlist.Length - 1)
        {
            nextTrackIndex = 0;
        }

        return playlist[nextTrackIndex];
    }

    /// <summary> Container for lobby soundtrack information. </summary>
    /// <param name="Filename">Soundtrack filename.</param>
    /// <param name="NextTrackOn">Time (based on <see cref="IGameTiming.CurTime"/>) when this track is going to finish playing and next track have to be started.</param>
    /// <param name="MusicStreamEntityUid">
    /// EntityUid of launched soundtrack (from <see cref="SharedAudioSystem.PlayGlobal(string,Robust.Shared.Player.Filter,bool,System.Nullable{Robust.Shared.Audio.AudioParams})"/>).
    /// </param>
    private sealed record LobbySoundtrackInfo(string Filename, TimeSpan NextTrackOn, EntityUid MusicStreamEntityUid);
}

/// <summary>
/// Event of changing lobby soundtrack (or stopping lobby music - will pass null for <paramref name="SoundtrackFilename"/> in that case).
/// Is used by <see cref="ContentAudioSystem.LobbySoundtrackChanged"/> and <see cref="LobbyState.UpdateLobbySoundtrackInfo"/>.
/// </summary>
/// <param name="SoundtrackFilename">Filename of newly set soundtrack, or null if soundtrack playback is stopped.</param>
public sealed record LobbySoundtrackChangedEvent(string? SoundtrackFilename = null);
