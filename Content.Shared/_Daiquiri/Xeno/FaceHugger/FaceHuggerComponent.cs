using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Xeno.FaceHugger;

[RegisterComponent, NetworkedComponent]
public sealed partial class FaceHuggerComponent : Component
{
    [DataField]
    public string Slot = "mask";

    [DataField]
    public TimeSpan KnockdownTime = TimeSpan.FromSeconds(5);

    [DataField]
    public EntProtoId? SpawnPrototype = "MobAlienSlider";
}
