using Content.Client.Credits;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client.Info
{
    public sealed class DevInfoBanner : BoxContainer
    {
        private Slider? _seekSlider;
        private bool _seekRefreshing;
        private bool _seekUpdating;

        protected override void EnteredTree()
        {
            base.EnteredTree();
            _seekRefreshing = true;
            RefreshSeek();
        }

        protected override void ExitedTree()
        {
            base.ExitedTree();
            _seekRefreshing = false;
        }

        private void RefreshSeek()
        {
            if (!_seekRefreshing || _seekSlider is not { Disposed: false })
                return;

            var audio = IoCManager.Resolve<IEntitySystemManager>()
                .GetEntitySystem<Content.Client.Audio.ContentAudioSystem>();
            var length = audio.LobbyTrackLengthSeconds;
            _seekSlider.Disabled = length is not > 0f;
            if (length is > 0f)
            {
                // Guard against MaxValue clamping firing a spurious seek.
                _seekUpdating = true;
                _seekSlider.MaxValue = length.Value;
                _seekSlider.SetValueWithoutEvent(audio.LobbyTrackPositionSeconds ?? 0f);
                _seekUpdating = false;
            }
            Timer.Spawn(500, RefreshSeek);
        }

        public DevInfoBanner() {
            var buttons = new BoxContainer
            {
                Orientation = LayoutOrientation.Horizontal
            };
            AddChild(buttons);

            var uriOpener = IoCManager.Resolve<IUriOpener>();
            var cfg = IoCManager.Resolve<IConfigurationManager>();

            var bugReport = cfg.GetCVar(CCVars.InfoLinksBugReport);
            if (bugReport != "")
            {
                var reportButton = new Button {Text = Loc.GetString("server-info-report-button")};
                reportButton.OnPressed += args => uriOpener.OpenUri(bugReport);
                buttons.AddChild(reportButton);
            }

            var creditsButton = new Button {Text = Loc.GetString("server-info-credits-button")};
            creditsButton.OnPressed += args => new CreditsWindow().Open();
            buttons.AddChild(creditsButton);

            // Daiquiri: small lobby music play/pause button right after authors.
            var musicButton = new Button { Text = "■", MinWidth = 32, Margin = new Thickness(6, 0, 0, 0) };
            musicButton.ToolTip = Loc.GetString("ui-lobby-music-pause-tooltip");
            musicButton.OnPressed += _ =>
            {
                var audio = IoCManager.Resolve<IEntitySystemManager>()
                    .GetEntitySystem<Content.Client.Audio.ContentAudioSystem>();
                audio.SetLobbyMusicPaused(!audio.LobbyMusicPaused);
                musicButton.Text = audio.LobbyMusicPaused ? "▶" : "■";
            };
            // Daiquiri: lobby track seek bar right after the pause button.
            buttons.AddChild(musicButton);
            var seekSlider = new Slider
            {
                MinValue = 0,
                MaxValue = 100,
                MinWidth = 100,
                ToolTip = Loc.GetString("ui-lobby-music-seek-tooltip"),
            };
            seekSlider.OnValueChanged += _ =>
            {
                if (_seekUpdating)
                    return;
                var audioSeek = IoCManager.Resolve<IEntitySystemManager>()
                    .GetEntitySystem<Content.Client.Audio.ContentAudioSystem>();
                audioSeek.SeekLobbyTrack(seekSlider.Value);
            };
            buttons.AddChild(seekSlider);
            _seekSlider = seekSlider;
        }
    }
}
