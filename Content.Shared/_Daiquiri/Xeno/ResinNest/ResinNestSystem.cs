using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Content.Shared.StatusEffectNew;

namespace Content.Shared._Daiquiri.Xeno.ResinNest;

public sealed partial class ResinNestSystem : EntitySystem
{
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    [Dependency] private readonly SleepingSystem _sleeping = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ResinNestComponent, StrappedEvent>(OnStrapped);
        SubscribeLocalEvent<ResinNestComponent, UnstrappedEvent>(OnUnstrapped);
    }

    private void OnStrapped(Entity<ResinNestComponent> nest, ref StrappedEvent args)
    {
        var victim = args.Buckle.Owner;
        _status.TrySetStatusEffectDuration(victim, SleepingSystem.StatusEffectForcedSleeping, out _, null);
        _sleeping.TrySleeping(victim);
    }

    private void OnUnstrapped(Entity<ResinNestComponent> nest, ref UnstrappedEvent args)
    {
        var victim = args.Buckle.Owner;
        _status.TryRemoveStatusEffect(victim, SleepingSystem.StatusEffectForcedSleeping);
        _sleeping.TryWaking(victim, force: true);
    }
}
