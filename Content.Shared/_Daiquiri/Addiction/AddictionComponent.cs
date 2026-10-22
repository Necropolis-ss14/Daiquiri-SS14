using System.Linq;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared._Daiquiri.Addiction;

/// <summary>
/// Marks an entity as dependent on one or more reagents. The withdrawal logic itself lives in the
/// server side <c>AddictionSystem</c>; the component only carries the state, so it can be filled in
/// from prototypes (traits, entities, ...).
/// </summary>
[RegisterComponent]
public sealed partial class AddictionComponent : Component
{
    /// <summary>
    /// Addictions of this entity.
    /// </summary>
    [DataField]
    public List<ProtoId<AddictionPrototype>> Addictions = new();

    /// <summary>
    /// When the entity last took a dose of each addiction.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<ProtoId<AddictionPrototype>, TimeSpan> LastDose = new();

    /// <summary>
    /// When the next withdrawal message is due for each addiction.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<ProtoId<AddictionPrototype>, TimeSpan> NextMessage = new();

    /// <summary>
    /// Which withdrawal phase was active the last time the addiction was processed, so that a new
    /// phase can announce itself right away.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<ProtoId<AddictionPrototype>, TimeSpan> LastStage = new();

    /// <summary>
    /// When the next forced emote is due for each addiction.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<ProtoId<AddictionPrototype>, TimeSpan> NextEmote = new();

    /// <summary>
    /// Time since last dose for each addiction, formatted for ViewVariables.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string TimeSinceLastDosePerAddiction
    {
        get
        {
            if (LastDose.Count == 0)
                return "None";

            var now = IoCManager.Resolve<IGameTiming>().CurTime;
            var sb = new System.Text.StringBuilder();
            foreach (var (id, lastDose) in LastDose)
            {
                var elapsed = now - lastDose;
                sb.Append($"{id}: {elapsed:g}; ");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// How much of each addiction's reagents has been absorbed in total, so that an addiction can
    /// appear on its own once the prototype's threshold is reached.
    /// </summary>
    public readonly Dictionary<ProtoId<AddictionPrototype>, FixedPoint2> Intake = new();

    /// <summary>
    /// Movement speed penalty that is currently applied for each addiction, as a fraction of the
    /// normal speed. Kept so that the penalty is only refreshed when it actually changes.
    /// </summary>
    public readonly Dictionary<ProtoId<AddictionPrototype>, float> Slowdown = new();

    /// <summary>
    /// Whether tremor (shaking) is currently active for each addiction.
    /// </summary>
    public readonly Dictionary<ProtoId<AddictionPrototype>, bool> Tremor = new();

    /// <summary>
    /// When the next forced tremor emote is due for each addiction.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<ProtoId<AddictionPrototype>, TimeSpan> NextTremor = new();

    /// <summary>
    /// Amount of every tracked reagent that was last seen in the bloodstream. The difference to the
    /// current amount is what the character has just absorbed.
    /// </summary>
    public readonly Dictionary<string, FixedPoint2> LastSeenReagents = new();

    /// <summary>
    /// Reagents that were metabolized since the last intake update. They no longer show up in the
    /// bloodstream, but they still count as absorbed, which keeps a slow dose (a cigarette) from
    /// being cancelled out by the metabolism.
    /// </summary>
    public readonly Dictionary<string, FixedPoint2> MetabolizedReagents = new();
}
