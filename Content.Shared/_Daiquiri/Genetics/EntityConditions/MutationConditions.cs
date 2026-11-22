using System.Linq;
using Content.Shared.EntityConditions;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Standing;
using Content.Shared.Timing;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityConditions;

public sealed partial class HoldingItemCondition : EntityConditionBase<HoldingItemCondition>
{
    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => string.Empty;
}

public sealed partial class HoldingItemConditionSystem : EntityConditionSystem<HandsComponent, HoldingItemCondition>
{
    [Dependency] private SharedHandsSystem _hands = default!;

    protected override void Condition(Entity<HandsComponent> ent, ref EntityConditionEvent<HoldingItemCondition> args)
    {
        args.Result = _hands.EnumerateHeld(ent.AsNullable()).Any();
    }
}

public sealed partial class StandingCondition : EntityConditionBase<StandingCondition>
{
    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => string.Empty;
}

public sealed partial class StandingConditionSystem : EntityConditionSystem<StandingStateComponent, StandingCondition>
{
    protected override void Condition(Entity<StandingStateComponent> ent, ref EntityConditionEvent<StandingCondition> args)
    {
        args.Result = ent.Comp.Standing;
    }
}

public sealed partial class UseDelayCondition : EntityConditionBase<UseDelayCondition>
{
    [DataField]
    public string DelayId = UseDelaySystem.DefaultId;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => string.Empty;
}

public sealed partial class UseDelayConditionSystem : EntityConditionSystem<UseDelayComponent, UseDelayCondition>
{
    [Dependency] private UseDelaySystem _useDelay = default!;

    protected override void Condition(Entity<UseDelayComponent> ent, ref EntityConditionEvent<UseDelayCondition> args)
    {
        args.Result = !_useDelay.IsDelayed((ent.Owner, ent.Comp), args.Condition.DelayId);
    }
}

public sealed partial class InContainerCondition : EntityConditionBase<InContainerCondition>
{
    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => string.Empty;
}

public sealed partial class InContainerConditionSystem : EntityConditionSystem<TransformComponent, InContainerCondition>
{
    [Dependency] private SharedContainerSystem _container = default!;

    protected override void Condition(Entity<TransformComponent> ent, ref EntityConditionEvent<InContainerCondition> args)
    {
        args.Result = _container.TryGetContainingContainer(ent.Owner, out _);
    }
}