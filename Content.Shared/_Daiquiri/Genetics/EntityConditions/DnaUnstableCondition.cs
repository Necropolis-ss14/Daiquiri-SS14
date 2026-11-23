using Content.Shared._Daiquiri.Genetics.Mutations;
using Content.Shared.EntityConditions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityConditions;

public sealed partial class DnaUnstableCondition : EntityConditionBase<DnaUnstableCondition>
{
    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
        => Loc.GetString("entity-condition-guidebook-dna-unstable");
}

public sealed class DnaUnstableConditionSystem : EntityConditionSystem<MutatableComponent, DnaUnstableCondition>
{
    protected override void Condition(Entity<MutatableComponent> ent, ref EntityConditionEvent<DnaUnstableCondition> args)
    {
        args.Result = ent.Comp.TotalInstability >= ent.Comp.MaxInstability;
    }
}
