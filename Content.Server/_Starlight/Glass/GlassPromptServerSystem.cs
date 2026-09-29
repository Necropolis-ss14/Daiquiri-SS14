using Content.Shared._Starlight.Glass;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server._Starlight.Glass;

/// <summary>
/// Registers the glass prompt network message on the server side.
/// </summary>
public sealed partial class GlassPromptServerSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>();
    }
}
