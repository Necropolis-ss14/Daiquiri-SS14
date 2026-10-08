using System;
using System.Linq;
using Content.Client.Audio;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby.UI;
using Content.Client.Message;
using Content.Client.Options.UI; // Daiquiri glass prompt
using Content.Client.Playtime;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.Voting;
using Content.Shared.CCVar;
using Content.Shared.GameTicking.Prototypes;
using Content.Shared._Starlight.CCVar; // Daiquiri glass prompt
using Robust.Client;
using Robust.Client.Console;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

#region Starlight

using Robust.Client.UserInterface.CustomControls;
using Content.Client._Starlight.Lobby.UI;
#endregion Starlight

namespace Content.Client.Lobby
{
    public sealed partial class LobbyState : Robust.Client.State.State
    {
        [Dependency] private IBaseClient _baseClient = default!;
        [Dependency] private IConfigurationManager _cfg = default!;
        [Dependency] private IClientConsoleHost _consoleHost = default!;
        [Dependency] private IEntityManager _entityManager = default!;
        [Dependency] private IResourceCache _resourceCache = default!;
        [Dependency] private IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private IGameTiming _gameTiming = default!;
        [Dependency] private IVoteManager _voteManager = default!;
        [Dependency] private ClientsidePlaytimeTrackingManager _playtimeTracking = default!;
        [Dependency] private IClientPreferencesManager _preferences = default!; //Starlight
        [Dependency] private IPrototypeManager _protoMan = default!;

        private ClientGameTicker _gameTicker = default!;
        private ContentAudioSystem _contentAudioSystem = default!;
        private LobbyMusicQueueWindow? _queueWindow;

        protected override Type? LinkedScreenType { get; } = typeof(LobbyGui);
        public LobbyGui? Lobby;

        private bool _readyPossibleWithCharacters;

        protected override void Startup()
        {
            if (_userInterfaceManager.ActiveScreen == null)
            {
                return;
            }

            Lobby = (LobbyGui) _userInterfaceManager.ActiveScreen;

            var chatController = _userInterfaceManager.GetUIController<ChatUIController>();
            _gameTicker = _entityManager.System<ClientGameTicker>();
            _contentAudioSystem = _entityManager.System<ContentAudioSystem>();
            _contentAudioSystem.LobbySoundtrackChanged += UpdateLobbySoundtrackInfo;
            _contentAudioSystem.LobbyPlaylistChanged += RefreshLobbyTrackLabel;

            chatController.SetMainChat(true);

            _voteManager.SetPopupContainer(Lobby.VoteContainer);
            LayoutContainer.SetAnchorPreset(Lobby, LayoutContainer.LayoutPreset.Wide);


            // STARLIGHT-START
            //Lobby.ServerName.Text = string.IsNullOrEmpty(lobbyNameCvar)
            //    ? Loc.GetString("ui-lobby-title", ("serverName", serverName))
            //    : lobbyNameCvar; //Moved to Lobby GUI
            // STARLIGHT-END

            var width = _cfg.GetCVar(CCVars.ServerLobbyRightPanelWidth);
            Lobby.RightSide.SetWidth = width;

            UpdateLobbyUi();

            Lobby.CharacterPreview.CharacterSetupButton.OnPressed += OnSetupPressed;
            Lobby.MusicSwitcher.PrevButton.OnPressed += OnMusicPrevPressed;
            Lobby.MusicSwitcher.NextButton.OnPressed += OnMusicNextPressed;
            Lobby.MusicSwitcher.QueueButton.OnPressed += OnMusicQueuePressed;
            Lobby.CharacterPreview.PrioritiesUpdated += UpdateReadyAllowed;
            Lobby.ReadyButton.OnPressed += OnReadyPressed;
            Lobby.ReadyButton.OnToggled += OnReadyToggled;
            Lobby.ReadyButton.TooltipSupplier = GetReadyButtonTooltip;

            _gameTicker.InfoBlobUpdated += UpdateLobbyUi;
            _gameTicker.LobbyStatusUpdated += LobbyStatusUpdated;
            _gameTicker.LobbyLateJoinStatusUpdated += LobbyLateJoinStatusUpdated;

            _userInterfaceManager.GetUIController<LobbyUIController>().OnAnyCharacterOrJobChange += UpdateReadyAllowed;

            // Daiquiri: first-run liquid glass prompt with live preview.
            // Shown only to newcomers (no characters yet), veterans are marked silently.
            if (!_cfg.GetCVar(StarlightCCVars.UIGlassThemeSeen))
            {
                if (_preferences.ServerDataLoaded)
                    MaybeShowGlassPrompt();
                else
                    _preferences.OnServerDataLoaded += OnPrefsLoadedForGlassPrompt;
            }

            // We need to set the disabled state of the ready button when the preferences are loaded...
            // Check for the case that they're already loaded!
            if(_preferences.ServerDataLoaded)
                UpdateReadyAllowed();
            _preferences.OnServerDataLoaded += UpdateReadyAllowed;
        }

        private void OnPrefsLoadedForGlassPrompt()
        {
            _preferences.OnServerDataLoaded -= OnPrefsLoadedForGlassPrompt;
            MaybeShowGlassPrompt();
        }

        private void MaybeShowGlassPrompt()
        {
            if (_cfg.GetCVar(StarlightCCVars.UIGlassThemeSeen))
                return;

            // Veterans (already have characters) never see the prompt.
            if ((_preferences.Preferences?.Characters.Count ?? 0) != 0)
            {
                _cfg.SetCVar(StarlightCCVars.UIGlassThemeSeen, true);
                return;
            }

            var glassPrompt = _userInterfaceManager.CreateWindow<GlassThemePrompt>();
            glassPrompt.OpenCentered();

            // Newcomers get the timefracture track with the prompt.
            _glassTrackSwitchPending = true;
            TrySwitchNewcomerTrack();
        }

        private bool _glassTrackSwitchPending;

        private void TrySwitchNewcomerTrack()
        {
            var track = _contentAudioSystem.LobbyPlaylistTracks
                ?.FirstOrDefault(f => f.Contains("timefracture", StringComparison.OrdinalIgnoreCase));
            if (track == null || _contentAudioSystem.CurrentLobbyTrack == track)
                return;

            _contentAudioSystem.PlayLobbyTrack(track);
        }

        private string GetReadyButtonTooltipText()
        {
            if (!Lobby!.ReadyButton.ToggleMode)
                return Loc.GetString("ui-lobby-ready-button-tooltip-join-state");
            if (!_preferences.ServerDataLoaded)
                return Loc.GetString("ui-lobby-ready-button-tooltip-not-loaded");
            if (!_readyPossibleWithCharacters)
                return Loc.GetString("ui-lobby-ready-button-tooltip-no-possible-characters");
            if (Lobby!.ReadyButton.Pressed)
                return Loc.GetString("ui-lobby-ready-button-tooltip-is-ready");
            return Loc.GetString("ui-lobby-ready-button-tooltip-is-not-ready");
        }

        private Control GetReadyButtonTooltip(Control sender)
        {
            return new Tooltip { Text = GetReadyButtonTooltipText() };
        }

        protected override void Shutdown()
        {
            var chatController = _userInterfaceManager.GetUIController<ChatUIController>();
            chatController.SetMainChat(false);
            _gameTicker.InfoBlobUpdated -= UpdateLobbyUi;
            _gameTicker.LobbyStatusUpdated -= LobbyStatusUpdated;
            _gameTicker.LobbyLateJoinStatusUpdated -= LobbyLateJoinStatusUpdated;
            _contentAudioSystem.LobbySoundtrackChanged -= UpdateLobbySoundtrackInfo;
            _contentAudioSystem.LobbyPlaylistChanged -= RefreshLobbyTrackLabel;
            _preferences.OnServerDataLoaded -= OnPrefsLoadedForGlassPrompt;

            _voteManager.ClearPopupContainer();

            Lobby!.CharacterPreview.CharacterSetupButton.OnPressed -= OnSetupPressed;
            Lobby!.MusicSwitcher.PrevButton.OnPressed -= OnMusicPrevPressed;
            Lobby!.MusicSwitcher.NextButton.OnPressed -= OnMusicNextPressed;
            Lobby!.MusicSwitcher.QueueButton.OnPressed -= OnMusicQueuePressed;
            Lobby!.ReadyButton.OnPressed -= OnReadyPressed;
            Lobby!.ReadyButton.OnToggled -= OnReadyToggled;

            Lobby = null;
        }

        public void SwitchState(LobbyGui.LobbyGuiState state)
        {
            // Yeah I hate this but LobbyState contains all the badness for now.
            Lobby?.SwitchState(state);
        }

        private void OnSetupPressed(BaseButton.ButtonEventArgs args)
        {
            SetReady(false);
            Lobby?.SwitchState(LobbyGui.LobbyGuiState.CharacterSetup);
        }

        private void OnReadyPressed(BaseButton.ButtonEventArgs args)
        {
            if (!_gameTicker.IsGameStarted)
            {
                return;
            }

            new LateJoinGui().OpenCentered();
        }

        private void OnReadyToggled(BaseButton.ButtonToggledEventArgs args)
        {
            SetReady(args.Pressed);
        }

        public override void FrameUpdate(FrameEventArgs e)
        {
            if (_gameTicker.IsGameStarted)
            {
                Lobby!.StartTime.Text = string.Empty;
                var roundTime = _gameTiming.CurTime.Subtract(_gameTicker.RoundStartTimeSpan);
                Lobby!.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", ("hours", roundTime.Hours), ("minutes", roundTime.Minutes));
                return;
            }

            Lobby!.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
            string text;

            if (_gameTicker.Paused)
            {
                text = Loc.GetString("lobby-state-paused");
            }
            else if (_gameTicker.StartTime < _gameTiming.CurTime)
            {
                Lobby!.StartTime.Text = Loc.GetString("lobby-state-soon");
                return;
            }
            else
            {
                var difference = _gameTicker.StartTime - _gameTiming.CurTime;
                var seconds = difference.TotalSeconds;
                if (seconds < 0)
                {
                    text = Loc.GetString(seconds < -5 ? "lobby-state-right-now-question" : "lobby-state-right-now-confirmation");
                }
                else if (difference.TotalHours >= 1)
                {
                    text = $"{Math.Floor(difference.TotalHours)}:{difference.Minutes:D2}:{difference.Seconds:D2}";
                }
                else
                {
                    text = $"{difference.Minutes}:{difference.Seconds:D2}";
                }
            }

            Lobby!.StartTime.Text = Loc.GetString("lobby-state-round-start-countdown-text", ("timeLeft", text));
        }

        private void LobbyStatusUpdated()
        {
            UpdateLobbyBackground();
            UpdateLobbyUi();
        }

        private void LobbyLateJoinStatusUpdated()
        {
            Lobby!.ReadyButton.Disabled = _gameTicker.DisallowedLateJoin;
        }

        private void UpdateLobbyUi()
        {
            if (_gameTicker.IsGameStarted)
            {
                Lobby!.ReadyButton.Text = Loc.GetString("lobby-state-ready-button-join-state");
                Lobby!.ReadyButton.ToggleMode = false;
                Lobby!.ReadyButton.Pressed = false;
                Lobby!.ReadyButton.Disabled = false;
                Lobby!.ObserveButton.Disabled = false;
            }
            else
            {
                Lobby!.StartTime.Text = string.Empty;
                Lobby!.ReadyButton.Pressed = _gameTicker.AreWeReady;
                Lobby!.ReadyButton.Text = Loc.GetString(Lobby!.ReadyButton.Pressed ? "lobby-state-player-status-ready": "lobby-state-player-status-not-ready");
                // If there is a tooltip showing, make sure to update the text in it as well!
                if (Lobby!.ReadyButton.SuppliedTooltip is Tooltip tooltip)
                {
                    tooltip.Text = GetReadyButtonTooltipText();
                }
                Lobby!.ReadyButton.ToggleMode = true;
                Lobby!.ReadyButton.Disabled = !_readyPossibleWithCharacters; // Starlight - false -> !_readyPossibleWithCharacters
                Lobby!.ObserveButton.Disabled = true;
            }

            if (_gameTicker.ServerInfoBlob != null)
            {
                Lobby!.ServerInfo.SetInfoBlob(_gameTicker.ServerInfoBlob);
            }

            var minutesToday = _playtimeTracking.PlaytimeMinutesToday;
            if (minutesToday > 60)
            {
                Lobby!.PlaytimeComment.Visible = true;

                var hoursToday = Math.Round(minutesToday / 60f, 1);

                var chosenString = minutesToday switch
                {
                    < 180 => "lobby-state-playtime-comment-normal",
                    < 360 => "lobby-state-playtime-comment-concerning",
                    < 720 => "lobby-state-playtime-comment-grasstouchless",
                    _ => "lobby-state-playtime-comment-selfdestructive"
                };

                Lobby.PlaytimeComment.SetMarkup(Loc.GetString(chosenString, ("hours", hoursToday)));
            }
            else
                Lobby!.PlaytimeComment.Visible = false;
        }

        private void UpdateLobbySoundtrackInfo(LobbySoundtrackChangedEvent ev)
        {
            // Playlist may arrive after the newcomer prompt - retry the track switch once.
            if (_glassTrackSwitchPending)
            {
                _glassTrackSwitchPending = false;
                TrySwitchNewcomerTrack();
            }

            if (ev.SoundtrackFilename == null)
            {
                SetLobbyTrackLabel(null);
            }
            else if (
                ev.SoundtrackFilename != null
                && _resourceCache.TryGetResource<AudioResource>(ev.SoundtrackFilename, out var lobbySongResource)
                )
            {
                SetLobbyTrackLabel(ev.SoundtrackFilename);
            }
        }

        /// <summary>
        /// Daiquiri: refresh the lobby track label from local state
        /// (pool changes don't fire LobbySoundtrackChanged).
        /// </summary>
        private void RefreshLobbyTrackLabel()
        {
            if (Lobby == null)
                return;
            var poolEmpty = _contentAudioSystem.LobbyPlaylistTracks == null;
            SetLobbyTrackLabel(poolEmpty ? null : _contentAudioSystem.CurrentLobbyTrack);
        }

        private void SetLobbyTrackLabel(string? filename)
        {
            if (filename == null)
            {
                // Daiquiri: empty queue shows the fallback instead of a track name.
                Lobby!.LobbySong.SetMarkup(Loc.GetString("lobby-state-song-no-song-text"));
                Lobby!.MusicSwitcher.TrackLabel.FullText = "Ничего не выбрано";
                return;
            }

            if (!_resourceCache.TryGetResource<AudioResource>(filename, out var lobbySongResource))
                return;

            // Daiquiri: prefer the prototype title, fall back to OGG metadata.
            var lobbyStream = lobbySongResource.AudioStream;
            var title = _protoMan.TryIndex<LobbyTrackPrototype>(filename, out var trackProto)
                ? trackProto.Title
                : string.IsNullOrEmpty(lobbyStream.Title)
                    ? Loc.GetString("lobby-state-song-unknown-title")
                    : lobbyStream.Title;

            var artist = string.IsNullOrEmpty(lobbyStream.Artist)
                ? Loc.GetString("lobby-state-song-unknown-artist")
                : lobbyStream.Artist;

            var markup = Loc.GetString("lobby-state-song-text",
                ("songTitle", title),
                ("songArtist", artist));

            Lobby!.LobbySong.SetMarkup(markup);
            Lobby!.MusicSwitcher.TrackLabel.FullText = $"{title} — {artist}";
        }

        private void OnMusicPrevPressed(BaseButton.ButtonEventArgs args)
        {
            _contentAudioSystem.PlayPreviousLobbyTrack();
        }

        private void OnMusicNextPressed(BaseButton.ButtonEventArgs args)
        {
            _contentAudioSystem.PlayNextLobbyTrack();
        }

        private void OnMusicQueuePressed(BaseButton.ButtonEventArgs args)
        {
            // Daiquiri: single queue window — reopening closes the old one.
            if (_queueWindow is { Disposed: false } existing)
            {
                if (existing.IsOpen)
                {
                    existing.Close();
                    return;
                }
            }
            _queueWindow = _userInterfaceManager.CreateWindow<LobbyMusicQueueWindow>();
            _queueWindow.OpenCentered();
        }

        private void UpdateLobbyBackground()
        {
            if (_protoMan.TryIndex(_gameTicker.LobbyBackground, out var proto))
            {
                Lobby!.Background.Texture = _resourceCache.GetResource<TextureResource>(proto.Background);

                var markup = Loc.GetString("lobby-state-background-text",
                    ("backgroundTitle", Loc.GetString(proto.Title)),
                    ("backgroundArtist", Loc.GetString(proto.Artist)));

                Lobby!.LobbyBackground.SetMarkup(markup);
            }
            else
            {
                Lobby!.Background.Texture = null;

                Lobby!.LobbyBackground.SetMarkup(Loc.GetString("lobby-state-background-no-background-text"));
            }
        }

        private void SetReady(bool newReady)
        {
            if (_gameTicker.IsGameStarted)
            {
                return;
            }

            _consoleHost.ExecuteCommand($"toggleready {newReady}");
        }

        /// <summary>
        /// Check the client preferences to make sure that it's possible to be considered for any round start jobs
        /// given the player's character selections and job priorities.
        /// Save and cache this result, and set the disable status and unready if appropriate.
        /// </summary>
        private void UpdateReadyAllowed()
        {
            if (!_preferences.ServerDataLoaded)
                return;

            _readyPossibleWithCharacters = (_preferences.Preferences?.JobPrioritiesFiltered().Count ?? 0) != 0;

            if (Lobby is null)
                return;

            // If the button is not in "Toggle Mode", the button is being used as a late join button instead, so bail.
            if (!Lobby.ReadyButton.ToggleMode)
                return;

            Lobby.ReadyButton.Disabled = !_readyPossibleWithCharacters;

            if (!_readyPossibleWithCharacters)
            {
                Lobby.ReadyButton.Pressed = false;
            }

            SetReady(Lobby.ReadyButton.Pressed);
        }
    }
}
