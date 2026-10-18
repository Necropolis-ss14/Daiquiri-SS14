using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Addiction;

[RegisterComponent]
public sealed partial class AddictionComponent : Component
{
    [DataField]
    public List<ProtoId<AddictionPrototype>> Addictions = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, TimeSpan> LastDose = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, TimeSpan> NextMessage = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, TimeSpan> LastStage = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, TimeSpan> NextEmote = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, FixedPoint2> Intake = new();

    public readonly Dictionary<ProtoId<AddictionPrototype>, float> Slowdown = new();

    public readonly Dictionary<string, FixedPoint2> LastSeenReagents = new();

    public readonly Dictionary<string, FixedPoint2> MetabolizedReagents = new();
}
