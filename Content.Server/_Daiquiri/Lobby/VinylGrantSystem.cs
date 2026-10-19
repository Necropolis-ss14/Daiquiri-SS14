using Content.Shared._Daiquiri.Lobby;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Daiquiri.Lobby;

/// <summary>
/// Daiquiri
/// Рассылает игрокам список выданных им пластинок.
/// </summary>
public sealed partial class VinylGrantSystem : EntitySystem
{
    [Dependency] private IVinylGrants _grants = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IServerNetManager _netMgr = default!;

    public override void Initialize()
    {
        base.Initialize();

        _netMgr.RegisterNetMessage<MsgVinylGrants>();
        _players.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _players.PlayerStatusChanged -= OnPlayerStatusChanged;
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        if (e.NewStatus == SessionStatus.Connected)
            SendGrants(e.Session);
    }

    public void Grant(ICommonSession session, string vinylId)
    {
        _grants.Grant(session.UserId, vinylId);
        SendGrants(session);
    }

    private void SendGrants(ICommonSession session)
    {
        var msg = new MsgVinylGrants
        {
            Vinyls = _grants.GetGrants(session.UserId),
        };

        _netMgr.ServerSendMessage(msg, session.Channel);
    }
}
