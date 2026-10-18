using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Addiction;

[Prototype]
public sealed partial class AddictionPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public LocId Name;

    [DataField(required: true)]
    public List<ProtoId<ReagentPrototype>> Reagents = new();

    [DataField]
    public List<ProtoId<ReagentPrototype>> Cures = new();

    [DataField]
    public FixedPoint2 Threshold = FixedPoint2.Zero;

    [DataField]
    public TimeSpan CureTime = TimeSpan.FromMinutes(40);

    [DataField]
    public List<AddictionStage> Stages = new();
}

[DataDefinition]
public sealed partial class AddictionStage
{
    [DataField(required: true)]
    public TimeSpan After;

    [DataField]
    public TimeSpan Interval = TimeSpan.FromMinutes(1);

    [DataField]
    public List<LocId> Messages = new();

    [DataField]
    public ProtoId<EmotePrototype>? Emote;

    [DataField]
    public TimeSpan EmoteInterval = TimeSpan.Zero;

    [DataField]
    public float Slowdown;

    [DataField]
    public Color Color = Color.FromHex("#b26ce0");
}
