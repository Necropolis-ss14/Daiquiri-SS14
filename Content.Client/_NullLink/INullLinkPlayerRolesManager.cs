
namespace Content.Client.Administration.Managers;

public interface INullLinkPlayerRolesManager
{
    event Action PlayerRolesChanged;

    bool ContainsAny(ulong[] roles);
    bool IsDiscordLinked { get; }
    string? GetDiscordLink();
    string? GetSteamLink();
    void Initialize();
}
