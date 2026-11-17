// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Timing;
using Content.Shared.Trigger;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._Lavaland.Compat;

[Serializable, NetSerializable]
public sealed partial class HarvestedDoAfterEvent : SimpleDoAfterEvent;

public sealed partial class HarvestableSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HarvestableComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<HarvestableComponent, HarvestedDoAfterEvent>(OnHarvestedDoAfter);
    }

    private void OnInteractHand(Entity<HarvestableComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager, args.User, ent.Comp.Delay, new HarvestedDoAfterEvent(), ent.Owner, ent.Owner)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            DistanceThreshold = 1.5f,
            RequireCanInteract = true,
            BlockDuplicate = true,
            CancelDuplicate = true,
            NeedHand = true,
        });
    }

    private void OnHarvestedDoAfter(Entity<HarvestableComponent> ent, ref HarvestedDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        Harvest(ent, args.User);
        args.Handled = true;
    }

    public void Harvest(Entity<HarvestableComponent> ent, EntityUid harvester)
    {
        var activeHand = _hands.GetActiveHand(harvester);

        if (ent.Comp.Loot != null)
        {
            var item = PredictedSpawnAtPosition(ent.Comp.Loot, Transform(harvester).Coordinates);
            _hands.TryPickup(harvester, item, activeHand, false);
        }

        PredictedDel(ent.Owner);
    }
}

[RegisterComponent, NetworkedComponent]
public sealed partial class HarvestableComponent : Component
{
    [DataField(required: true)]
    public EntProtoId? Loot;

    [DataField]
    public float Delay = 1f;
}

public sealed partial class LavalandTriggerCounterSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerCounterComponent, TriggerEvent>(OnTriggerCounter);
        SubscribeLocalEvent<TriggerCounterLimitComponent, AttemptTriggerEvent>(OnTriggerLimitCounter);
    }

    private void OnTriggerCounter(Entity<TriggerCounterComponent> ent, ref TriggerEvent args)
    {
        if (args.Handled)
            return;

        ent.Comp.Count++;
    }

    private void OnTriggerLimitCounter(Entity<TriggerCounterLimitComponent> ent, ref AttemptTriggerEvent args)
    {
        if (!TryComp(ent.Owner, out TriggerCounterComponent? counter))
            return;

        if (counter.Count >= ent.Comp.MaxCount)
            args.Cancelled = true;
    }
}

public sealed partial class HeldGrantSystem : EntitySystem
{
    [Dependency] private IComponentFactory _componentFactory = default!;
    [Dependency] private ISerializationManager _serializationManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HeldGrantComponent, GotEquippedHandEvent>(OnCompEquip);
        SubscribeLocalEvent<HeldGrantComponent, GotUnequippedHandEvent>(OnCompUnequip);
    }

    private void OnCompEquip(Entity<HeldGrantComponent> ent, ref GotEquippedHandEvent args)
    {
        foreach (var (name, data) in ent.Comp.Components)
        {
            var newComp = (Component) _componentFactory.GetComponent(name);
            if (HasComp(args.User, newComp.GetType()))
                continue;

            object? temp = newComp;
            _serializationManager.CopyTo(data.Component, ref temp);
            AddComp(args.User, (Component) temp!);

            ent.Comp.Active[name] = true;
        }
    }

    private void OnCompUnequip(Entity<HeldGrantComponent> ent, ref GotUnequippedHandEvent args)
    {
        foreach (var (name, _) in ent.Comp.Components)
        {
            if (!ent.Comp.Active.ContainsKey(name) || !ent.Comp.Active[name])
                continue;

            var newComp = (Component) _componentFactory.GetComponent(name);

            RemComp(args.User, newComp.GetType());
            ent.Comp.Active[name] = false;
        }
    }
}

public sealed partial class UseDelayBlockMeleeSystem : EntitySystem
{
    [Dependency] private UseDelaySystem _useDelay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UseDelayBlockMeleeComponent, AttemptMeleeEvent>(OnMeleeAttempt);
    }

    private void OnMeleeAttempt(Entity<UseDelayBlockMeleeComponent> ent, ref AttemptMeleeEvent args)
    {
        if (!TryComp(ent, out UseDelayComponent? useDelay))
            return;

        if (ent.Comp.Delays.Any(delay => _useDelay.IsDelayed((ent, useDelay), delay)))
            args.Cancelled = true;
    }
}
