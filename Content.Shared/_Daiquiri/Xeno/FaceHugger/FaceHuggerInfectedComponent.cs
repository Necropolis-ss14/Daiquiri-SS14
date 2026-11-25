using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Xeno.FaceHugger;

[RegisterComponent, NetworkedComponent]
public sealed partial class FaceHuggerInfectedComponent : Component
{
    [DataField]
    public EntProtoId SpawnPrototype = "MobAlienSlider";
}
