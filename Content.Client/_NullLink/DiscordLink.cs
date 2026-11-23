using Content.Client.Administration.Managers;
using Robust.Client.UserInterface;

namespace Content.Client._NullLink;

public static class DiscordLink
{
    public static bool IsAvailable => GetLink() != null;

    public static void Open()
    {
        var link = GetLink();
        if (link != null)
            IoCManager.Resolve<IUriOpener>().OpenUri(link);
    }

    private static string? GetLink()
    {
        var link = IoCManager.Resolve<INullLinkPlayerRolesManager>().GetDiscordLink();
        return string.IsNullOrEmpty(link) ? null : link;
    }
}
