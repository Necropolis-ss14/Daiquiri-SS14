using System;
using System.Linq;
using Content.Client.Audio;
using Content.Client.Options.UI;
using Content.Shared._Starlight.Glass;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Client._Starlight.Glass;

/// <summary>
/// Opens the glass prompt window when the server asks for it:
/// first-join trigger and glassshow command. Also switches the
/// newcomer to the timefracture lobby track.
/// </summary>
public sealed partial class GlassPromptSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;
    [Dependency] private IUserInterfaceManager _ui = default!;
    [Dependency] private ContentAudioSystem _audio = default!;

    private bool _newcomerTrackPending;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>();
        SubscribeNetworkEvent<GlassPromptShowEvent>(OnShowPromptEvent);
        _audio.LobbySoundtrackChanged += OnLobbySoundtrack;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _audio.LobbySoundtrackChanged -= OnLobbySoundtrack;
    }

    private void OnShowPromptEvent(GlassPromptShowEvent ev)
    {
        Logger.InfoS("glassprompt", "Received GlassPromptShowEvent, opening prompt window.");
        _ui.CreateWindow<GlassThemePrompt>().OpenCentered();

        // Daiquiri: newcomers get the timefracture track with the prompt.
        _newcomerTrackPending = true;
        TryNewcomerTrack();
    }

    private void OnShowPrompt(GlassPromptShowMessage message)
    {
        Logger.InfoS("glassprompt", "Received GlassPromptShowMessage, opening prompt window.");
        _ui.CreateWindow<GlassThemePrompt>().OpenCentered();
    }

    private void OnLobbySoundtrack(LobbySoundtrackChangedEvent ev)
    {
        // Playlist may arrive after the prompt - retry the track switch once.
        if (!_newcomerTrackPending)
            return;
        _newcomerTrackPending = false;
        TryNewcomerTrack();
    }

    private void TryNewcomerTrack()
    {
        var track = _audio.LobbyPlaylistTracks
            ?.FirstOrDefault(f => f.Contains("timefracture", StringComparison.OrdinalIgnoreCase));
        if (track == null || _audio.CurrentLobbyTrack == track)
            return;

        _audio.PlayLobbyTrack(track);
    }
}
