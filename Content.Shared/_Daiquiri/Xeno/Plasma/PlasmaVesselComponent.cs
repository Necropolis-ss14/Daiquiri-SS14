using Content.Shared.FixedPoint;
using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared._Daiquiri.Xeno.Plasma;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlasmaVesselComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 Plasma = 100;

    [DataField]
    public FixedPoint2 MaxPlasma = 250;

    [DataField]
    public FixedPoint2 PlasmaPerSecondOffWeed = 0.5f;

    [DataField]
    public FixedPoint2 PlasmaPerSecondOnWeed = 5f;

    [ViewVariables]
    public TimeSpan NextPointsAt;
}

public readonly record struct PlasmaAmountChangeEvent(FixedPoint2 Amount);

public sealed partial class TransferPlasmaActionEvent : EntityTargetActionEvent
{
    [DataField]
    public FixedPoint2 Amount = 50;
}
