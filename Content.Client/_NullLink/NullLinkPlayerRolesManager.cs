using System.Collections.Immutable;
using System.Linq;
using Content.Client.Administration.Managers;
using Content.Shared._NullLink;
using Robust.Shared.Network;

namespace Content.Client._NullLink;

public sealed partial class NullLinkPlayerRolesManager : INullLinkPlayerRolesManager
{
    [Dependency] private IClientNetManager _netMgr = default!;
    [Dependency] private ILogManager _logManager = default!;

    private ImmutableHashSet<ulong> _roles = [];
    private bool _discordLinked;
    private string? _discordLink;
    private string? _steamLink;
    private ISawmill _sawmill = default!;

    public event Action PlayerRolesChanged = delegate { };

    public void Initialize()
    {
        _sawmill = _logManager.GetSawmill("admin");
        _netMgr.RegisterNetMessage<MsgUpdatePlayerRoles>(Update);
    }

    private void Update(MsgUpdatePlayerRoles message)
    {
        _roles = message.Roles;
        _discordLinked = message.DiscordLinked;
        _discordLink = message.DiscordLink;
        _steamLink = message.SteamLink;

        _sawmill.Info("Updated player roles");
        PlayerRolesChanged?.Invoke();
    }

    public bool IsDiscordLinked => _discordLinked;

    public string? GetDiscordLink()
        => _discordLink;

    public string? GetSteamLink()
        => _steamLink;

    public bool ContainsAny(ulong[] roles)
        => roles.Any(_roles.Contains);
}
