using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Addiction;

/// <summary>
/// Describes a dependency (addiction) of a character.
/// The addiction is satisfied by one of the <see cref="Reagents"/>: while such a reagent is in the
/// character's bloodstream the character is considered to have taken their dose and the withdrawal
/// timer is reset. The longer the character stays clean, the further the withdrawal advances through
/// <see cref="Stages"/>, making the messages both more frequent and more aggressive. Staying clean
/// for <see cref="CureTime"/> cures the addiction on its own, and <see cref="Cures"/> can remove it
/// instantly.
/// </summary>
[Prototype]
public sealed partial class AddictionPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Localization key of the addiction's name. Shown to the character when the addiction is cured.
    /// </summary>
    [DataField(required: true)]
    public LocId Name;

    /// <summary>
    /// Reagents that satisfy the addiction. Taking any of them resets the withdrawal timer and
    /// returns the addiction to the first phase.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<ReagentPrototype>> Reagents = new();

    /// <summary>
    /// Reagents that remove this addiction instantly, no matter how long it has been active.
    /// </summary>
    [DataField]
    public List<ProtoId<ReagentPrototype>> Cures = new();

    /// <summary>
    /// How much of <see cref="Reagents"/> the character has to absorb in total before the addiction
    /// appears on its own. Zero or negative disables this, so the addiction can then only be gained
    /// from prototypes (traits, entities, ...). Every unit that enters the bloodstream counts, so a
    /// character slowly smoking or drinking their way to the threshold eventually gets hooked.
    /// </summary>
    [DataField]
    public FixedPoint2 Threshold = FixedPoint2.Zero;

    /// <summary>
    /// How long the character can stay clean before the addiction is cured automatically.
    /// Zero or negative disables the automatic cure.
    /// </summary>
    [DataField]
    public TimeSpan CureTime = TimeSpan.FromMinutes(40);

    /// <summary>
    /// Withdrawal phases. The most advanced phase whose <see cref="AddictionStage.After"/> has
    /// passed is the active one.
    /// </summary>
    [DataField]
    public List<AddictionStage> Stages = new();
}

/// <summary>
/// A single phase of a withdrawal. Messages of the active phase are sent to the character every
/// <see cref="Interval"/>, so later phases are both more aggressive and more frequent.
/// </summary>
[DataDefinition]
public sealed partial class AddictionStage
{
    /// <summary>
    /// Time since the last dose at which this phase starts.
    /// </summary>
    [DataField(required: true)]
    public TimeSpan After;

    /// <summary>
    /// How often messages are sent while this phase is active.
    /// </summary>
    [DataField]
    public TimeSpan Interval = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Localization keys of the messages that can be sent during this phase. One is picked at random.
    /// May be empty, for a phase that only forces an <see cref="Emote"/>.
    /// </summary>
    [DataField]
    public List<LocId> Messages = new();

    /// <summary>
    /// Emote the character is forced to perform while this phase is active, e.g. Cough for a smoker
    /// who has not had a cigarette in a while. The most advanced phase that has an emote wins.
    /// </summary>
    [DataField]
    public ProtoId<EmotePrototype>? Emote;

    /// <summary>
    /// How often <see cref="Emote"/> is performed while this phase is active.
    /// Zero means the phase's <see cref="Interval"/> is used.
    /// </summary>
    [DataField]
    public TimeSpan EmoteInterval = TimeSpan.Zero;

    /// <summary>
    /// Movement speed penalty of this phase, as a fraction of the normal speed: 0.1 slows the
    /// character down by 10%. The most advanced phase that has a penalty wins, and the penalty stays
    /// until the character takes their dose again. Zero means no penalty.
    /// </summary>
    [DataField]
    public float Slowdown;

    /// <summary>
    /// Chat color of the messages of this phase.
    /// </summary>
    [DataField]
    public Color Color = Color.FromHex("#b26ce0");
}
