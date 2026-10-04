using Content.Shared._Starlight.Glass;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Starlight.Glass;

/// <summary>
/// Sends the liquid glass prompt event to players (see glassshow command).
/// </summary>
public sealed partial class GlassPromptServerSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>();
    }

    public void ShowPromptTo(ICommonSession session)
    {
        RaiseNetworkEvent(new GlassPromptShowEvent(), session.Channel);
    }
}
