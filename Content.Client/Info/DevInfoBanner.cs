using Content.Client.Credits;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Info
{
    public sealed class DevInfoBanner : BoxContainer
    {
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
            var musicButton = new Button { Text = "■", MinWidth = 32 };
            musicButton.ToolTip = Loc.GetString("ui-lobby-music-pause-tooltip");
            musicButton.OnPressed += _ =>
            {
                var audio = IoCManager.Resolve<IEntitySystemManager>()
                    .GetEntitySystem<Content.Client.Audio.ContentAudioSystem>();
                audio.SetLobbyMusicPaused(!audio.LobbyMusicPaused);
                musicButton.Text = audio.LobbyMusicPaused ? "▶" : "■";
            };
            buttons.AddChild(musicButton);
        }
    }
}
