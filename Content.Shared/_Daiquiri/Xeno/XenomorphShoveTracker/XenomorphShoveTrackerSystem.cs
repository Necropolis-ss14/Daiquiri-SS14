using Content.Shared._Starlight.Antags.Xeno.Components;
using Content.Shared.CombatMode;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Robust.Shared.Timing;

namespace Content.Shared._Daiquiri.Xeno.XenomorphShoveTracker;

public sealed partial class XenomorphShoveTrackerSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedStunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MobStateComponent, DisarmedEvent>(OnDisarmed);
    }

    private void OnDisarmed(EntityUid uid, MobStateComponent mobStateComponent, ref DisarmedEvent args)
    {
        var source = args.Source;
        var target = args.Target;

        if (!HasComp<XenoComponent>(source) || !TryComp<XenomorphShoveTrackerComponent>(source, out var tracker))
            return;

        var currentTime = _timing.CurTime;

        CleanupOldShoves(tracker, currentTime);

        if (!tracker.ShoveCount.ContainsKey(target))
            tracker.ShoveCount[target] = 0;

        tracker.ShoveCount[target]++;
        tracker.LastShoveTime[target] = currentTime;

        if (tracker.ShoveCount[target] >= tracker.ShoveThreshold)
        {
            _stun.TryUpdateStunDuration(target, tracker.KnockdownDuration);
            _stun.TryKnockdown(target, tracker.KnockdownDuration, true);

            tracker.ShoveCount.Remove(target);
            tracker.LastShoveTime.Remove(target);
        }

        args.Handled = true;

        Dirty(source, tracker);
    }

    private static void CleanupOldShoves(XenomorphShoveTrackerComponent component, TimeSpan currentTime)
    {
        var toRemove = new List<EntityUid>();

        foreach (var (target, lastTime) in component.LastShoveTime)
        {
            if (currentTime - lastTime > component.ShoveResetTime)
                toRemove.Add(target);
        }

        foreach (var target in toRemove)
        {
            component.ShoveCount.Remove(target);
            component.LastShoveTime.Remove(target);
        }
    }
}
