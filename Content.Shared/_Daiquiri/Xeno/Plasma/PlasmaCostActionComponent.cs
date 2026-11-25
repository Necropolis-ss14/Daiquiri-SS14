using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Xeno.Plasma;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlasmaCostActionComponent : Component
{
    [DataField]
    public bool ShouldChangePlasma = true;

    [DataField, AutoNetworkedField]
    public FixedPoint2 PlasmaCost = 50;
}
