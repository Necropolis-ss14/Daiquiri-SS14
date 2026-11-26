using Content.Shared.Actions;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Components;
using Content.Shared.Standing;

namespace Content.Shared._Daiquiri.Sleeping;

public sealed partial class LyingSleepSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StandingStateComponent, DownedEvent>(OnDowned);
        SubscribeLocalEvent<StandingStateComponent, StoodEvent>(OnStood);
        SubscribeLocalEvent<LyingSleepComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnDowned(Entity<StandingStateComponent> ent, ref DownedEvent args)
    {
        if (!HasComp<HumanoidAppearanceComponent>(ent)
            || !HasComp<MobStateComponent>(ent)
            || HasComp<SleepingComponent>(ent)
            || TryComp<BuckleComponent>(ent, out var buckle) && buckle.Buckled)
            return;

        var comp = EnsureComp<LyingSleepComponent>(ent);
        if (comp.SleepAction == null)
            _actions.AddAction(ent, ref comp.SleepAction, SleepingSystem.SleepActionId);
    }

    private void OnStood(Entity<StandingStateComponent> ent, ref StoodEvent args)
    {
        RemComp<LyingSleepComponent>(ent);
    }

    private void OnShutdown(Entity<LyingSleepComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.SleepAction != null)
            _actions.RemoveAction(ent.Owner, ent.Comp.SleepAction);
    }
}
