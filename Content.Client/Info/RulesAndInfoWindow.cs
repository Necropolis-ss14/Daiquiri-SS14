using System.Numerics;
using Content.Client._NullLink;
using Content.Client.Administration.Managers;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.ContentPack;

namespace Content.Client.Info
{
    public sealed partial class RulesAndInfoWindow : DefaultWindow
    {
        [Dependency] private IResourceManager _resourceManager = default!;

        private readonly INullLinkPlayerRolesManager _playerRoles = IoCManager.Resolve<INullLinkPlayerRolesManager>();
        private Button? _discordButton;

        public RulesAndInfoWindow()
        {
            IoCManager.InjectDependencies(this);

            _playerRoles.PlayerRolesChanged += UpdateDiscordLinkButton;

            Title = Loc.GetString("ui-info-title");

            var rootContainer = new TabContainer();

            var rulesList = new RulesControl
            {
                Margin = new Thickness(10)
            };
            var tutorialList = new Info
            {
                Margin = new Thickness(10)
            };

            rootContainer.AddChild(rulesList);
            rootContainer.AddChild(tutorialList);

            TabContainer.SetTabTitle(rulesList, Loc.GetString("ui-info-tab-rules"));
            TabContainer.SetTabTitle(tutorialList, Loc.GetString("ui-info-tab-tutorial"));

            PopulateTutorial(tutorialList);

            ContentsContainer.AddChild(rootContainer);

            SetSize = new Vector2(650, 650);
        }

        private void PopulateTutorial(Info tutorialList)
        {
            AddSection(tutorialList, Loc.GetString("ui-info-header-intro"), "Intro.txt");
            var infoControlSection = new InfoControlsSection();
            tutorialList.InfoContainer.AddChild(infoControlSection);
            AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", true);
            AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", true);

            infoControlSection.ControlsButton.OnPressed += _ => UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow();

            var discordSection = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
            };
            discordSection.AddChild(new Label
            {
                Text = Loc.GetString("nulllink-discord-link-title"),
                StyleClasses = { "LabelHeading" },
            });
            var discordRow = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Horizontal,
                SeparationOverride = 5,
                HorizontalExpand = true,
            };
            var discordText = new RichTextLabel { HorizontalExpand = true };
            discordText.SetMessage(Loc.GetString("nulllink-discord-link-text"));
            discordRow.AddChild(discordText);
            var discordButton = new Button();
            DiscordLink.UpdateButton(discordButton);
            discordButton.OnPressed += _ => DiscordLink.Open();
            _discordButton = discordButton;
            discordRow.AddChild(discordButton);
            discordSection.AddChild(discordRow);
            tutorialList.InfoContainer.AddChild(discordSection);
        }

        private void UpdateDiscordLinkButton()
        {
            if (_discordButton != null && !Disposed)
                DiscordLink.UpdateButton(_discordButton);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _playerRoles.PlayerRolesChanged -= UpdateDiscordLinkButton;
        }

        private static void AddSection(Info info, Control control)
        {
            info.InfoContainer.AddChild(control);
        }

        private void AddSection(Info info, string title, string path, bool markup = false)
        {
            AddSection(info, MakeSection(title, path, markup, _resourceManager));
        }

        private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
        {
            return new InfoSection(title, res.ContentFileReadAllText($"/ServerInfo/{path}"), markup);
        }

    }
}
