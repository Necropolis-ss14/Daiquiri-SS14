using Content.Shared._Starlight.Antags.Xeno.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;

namespace Content.Shared._Daiquiri.Xeno.FaceHugger;

public sealed partial class FaceHuggerSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SleepingSystem _sleeping = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FaceHuggerComponent, StepTriggeredOffEvent>(OnStepTriggered);
        SubscribeLocalEvent<FaceHuggerComponent, BeingUnequippedAttemptEvent>(OnUnequipAttempt);
        SubscribeLocalEvent<FaceHuggerInfectedComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnStepTriggered(Entity<FaceHuggerComponent> hugger, ref StepTriggeredOffEvent args)
    {
        var target = args.Tripper;
        if (HasComp<XenoComponent>(target) || HasComp<FaceHuggerInfectedComponent>(target) ||
            !HasComp<MobStateComponent>(target))
            return;

        if (!_inventory.TryEquip(hugger, target, hugger, hugger.Comp.Slot, force: true))
            return;

        _stun.TryKnockdown(target, hugger.Comp.KnockdownTime, true);
        _status.TrySetStatusEffectDuration(target, SleepingSystem.StatusEffectForcedSleeping, out _, null);
        _sleeping.TrySleeping(target);

        var infected = EnsureComp<FaceHuggerInfectedComponent>(target);
        infected.SpawnPrototype = hugger.Comp.SpawnPrototype ?? "MobAlienSlider";
        Dirty(target, infected);
    }

    private void OnUnequipAttempt(Entity<FaceHuggerComponent> hugger, ref BeingUnequippedAttemptEvent args)
    {
        args.Cancel();
    }

    private void OnMobStateChanged(Entity<FaceHuggerInfectedComponent> victim, ref MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        var coordinates = Transform(victim.Owner).Coordinates;
        Spawn(victim.Comp.SpawnPrototype, coordinates);
        RemCompDeferred<FaceHuggerInfectedComponent>(victim.Owner);
    }
}
