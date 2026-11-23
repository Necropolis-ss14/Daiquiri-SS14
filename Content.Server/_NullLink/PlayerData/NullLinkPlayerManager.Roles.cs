using System.Collections.Immutable;
using System.Threading.Tasks;
using Content.Shared._NullLink;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Starlight.NullLink.Event;

namespace Content.Server._NullLink.PlayerData;

public sealed partial class NullLinkPlayerManager : INullLinkPlayerManager
{
    public ValueTask SyncRoles(PlayerRolesSyncEvent ev)
    {
        if (IsBridgeEnabled)
            return ValueTask.CompletedTask;

        if (!_playerById.TryGetValue(ev.Player, out var playerData))
            return ValueTask.CompletedTask;
        playerData.SyncRoles(ev);
        playerData.DiscordId = ev.DiscordId;

        MentorCheck(ev.Player, playerData);
        AdminCheck(ev.Player, playerData);

        RebuildTitle(_playerManager.GetSessionById(new NetUserId(ev.Player)), playerData);

        SendPlayerRoles(playerData.Session, playerData.Roles);
        return ValueTask.CompletedTask;
    }

    public ValueTask SetBridgeRoles(NetUserId player, ulong discordId, IEnumerable<ulong> roles)
    {
        if (!_playerById.TryGetValue(player.UserId, out var playerData))
            return ValueTask.CompletedTask;

        playerData.Roles = [.. roles];
        playerData.DiscordId = discordId;

        MentorCheck(player.UserId, playerData);
        AdminCheck(player.UserId, playerData);

        RebuildTitle(_playerManager.GetSessionById(player), playerData);

        if (discordId != 0)
        {
            var session = _playerManager.GetSessionById(player);
            if (session is not null)
                OnDiscordPromptClosed(session);
        }

        SendPlayerRoles(playerData.Session, playerData.Roles);
        return ValueTask.CompletedTask;
    }

    public bool IsDiscordLinked(Guid userId)
        => _playerById.TryGetValue(userId, out var playerData) && playerData.DiscordId != 0;

    public ValueTask UpdateRoles(RolesChangedEvent ev)
    {
        if (IsBridgeEnabled)
            return ValueTask.CompletedTask;

        if (!_playerById.TryGetValue(ev.Player, out var playerData))
            return ValueTask.CompletedTask;
        playerData.UpdateRoles(ev);
        playerData.DiscordId = ev.DiscordId;

        MentorCheck(ev.Player, playerData);
        AdminCheck(ev.Player, playerData);

        RebuildTitle(_playerManager.GetSessionById(new NetUserId(ev.Player)), playerData);

        SendPlayerRoles(playerData.Session, playerData.Roles);
        return ValueTask.CompletedTask;
    }

    private void SendPlayerRoles(ICommonSession session, ImmutableHashSet<ulong> roles)
    => _netMgr.ServerSendMessage(new MsgUpdatePlayerRoles
    {
        Roles = roles,
        DiscordLink = GetActiveDiscordAuthUrl(session.UserId.ToString()),
        SteamLink = GetSteamAuthUrl(session.UserId.ToString()),
    }, session.Channel);
}
