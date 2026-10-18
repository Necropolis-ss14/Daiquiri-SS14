using System.Text;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

// ReSharper disable once CheckNamespace
namespace Content.Shared.Roles;

[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class SpeciesComponentsRequirement : JobRequirement
{
    [DataField(required: true)]
    public HashSet<string> Components = new();

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

        if (profile is null)
            return true;

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
