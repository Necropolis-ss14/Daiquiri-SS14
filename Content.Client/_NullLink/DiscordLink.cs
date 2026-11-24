using Content.Client.Administration.Managers;
using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._NullLink;

public static class DiscordLink
{
    public static bool IsAvailable => GetLink() != null;
    public static bool IsLinked => IoCManager.Resolve<INullLinkPlayerRolesManager>().IsDiscordLinked;

    public static void Open()
    {
        var link = GetLink();
        if (link != null)
            IoCManager.Resolve<IUriOpener>().OpenUri(link);
    }

    public static void UpdateButton(Button button)
    {
        var linked = IsLinked;
        button.Visible = IsAvailable;
        button.Disabled = linked;
        button.Text = Loc.GetString(linked ? "server-info-discord-linked-button" : "server-info-connect-discord-button");

        if (linked)
            button.AddStyleClass(StyleNano.StyleClassButtonColorGreen);
        else
            button.RemoveStyleClass(StyleNano.StyleClassButtonColorGreen);
    }

    private static string? GetLink()
    {
        var link = IoCManager.Resolve<INullLinkPlayerRolesManager>().GetDiscordLink();
        return string.IsNullOrEmpty(link) ? null : link;
    }
}
