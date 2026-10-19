using System.Text;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

// ReSharper disable once CheckNamespace
namespace Content.Shared._Daiquiri.Roles;

/// <summary>
/// Requires the character's species to have certain components, for example a bloodstream, which is
/// where injected reagents end up. Useful for traits that only make sense for species that can
/// actually absorb substances.
/// </summary>
[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class SpeciesComponentsRequirement : JobRequirement
{
    /// <summary>
    /// Component names (as registered, e.g. Bloodstream) that the species' mob prototype must have.
    /// Prototype inheritance counts, so a component defined on a parent prototype is enough.
    /// </summary>
    [DataField(required: true)]
    public HashSet<string> Components = new();

    /// <summary>
    /// Localization key of the message shown when the character's species cannot absorb reagents.
    /// It also doubles as the message for an <see cref="JobRequirement.Inverted"/> requirement.
    /// </summary>
    [DataField]
    public LocId FailReason = "trait-requirement-species-cannot-absorb";

    public override bool Check(
        IEntityManager entManager,
        ICommonSession? player,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan>? playTimes,
        out FormattedMessage reason)
    {
        reason = new FormattedMessage();

        // Ghosts have no profile, so they should not be blocked by this.
        if (profile is null)
            return true;

        // An unknown species must not lock the trait away.
        if (!protoManager.TryIndex(profile.Species, out SpeciesPrototype? species))
            return true;

        var canAbsorb = false;

        if (protoManager.TryIndex(species.Prototype, out EntityPrototype? mob))
        {
            canAbsorb = true;

            foreach (var component in Components)
            {
                if (mob.Components.ContainsKey(component))
                    continue;

                canAbsorb = false;
                break;
            }
        }

        reason = FormattedMessage.FromMarkupPermissive(Loc.GetString(FailReason));

        return Inverted ? !canAbsorb : canAbsorb;
    }
}
