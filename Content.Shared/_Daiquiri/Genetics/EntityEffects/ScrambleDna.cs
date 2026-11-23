using Content.Shared.EntityEffects;
using Content.Shared.Humanoid;
using Content.Shared.Trigger.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityEffects;

public sealed partial class ScrambleDna : EntityEffectBase<ScrambleDna>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
        => loc.GetString("entity-effect-guidebook-scramble-dna", ("chance", Probability));
}

public sealed partial class ScrambleDnaEntityEffectSystem : EntityEffectSystem<HumanoidAppearanceComponent, ScrambleDna>
{
    [Dependency] private DnaScrambleOnTriggerSystem _scramble = default!;

    protected override void Effect(Entity<HumanoidAppearanceComponent> ent, ref EntityEffectEvent<ScrambleDna> args)
    {
        _scramble.Scramble(ent.Owner, ent.Comp);
    }
}
