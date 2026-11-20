using Content.Shared._Daiquiri.Harpy;
using Content.Shared.Actions;
using Content.Shared.Bed.Sleep;
using Content.Shared.Climbing.Events;
using Content.Shared.Cuffs.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Systems;
using Content.Shared.Gravity;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Mobs;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Physics;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Zombies;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;

namespace Content.Server._Daiquiri.Harpy;

/// <summary>
///     Temporary weightless flight at the cost of stamina and hand usage.
/// </summary>
public sealed partial class HarpyFlightSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actionsSystem = default!;
    [Dependency] private SharedVirtualItemSystem _virtualItem = default!;
    [Dependency] private SharedStaminaSystem _staminaSystem = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedPopupSystem _popupSystem = default!;
    [Dependency] private StandingStateSystem _standing = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private SharedGravitySystem _gravity = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FlightComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<FlightComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<FlightComponent, ToggleFlightEvent>(OnToggleFlight);
        SubscribeLocalEvent<FlightComponent, RefreshWeightlessModifiersEvent>(OnRefreshWeightlessMoveSpeed);
        SubscribeLocalEvent<FlightComponent, BeforeStaminaDamageEvent>(OnBeforeStaminaDamage);
        SubscribeLocalEvent<CuffableComponent, FlightAttemptEvent>(OnCuffableFlightAttempt);
        SubscribeLocalEvent<StandingStateComponent, FlightAttemptEvent>(OnStandingStateFlightAttempt);
        SubscribeLocalEvent<ZombieComponent, FlightAttemptEvent>(OnZombieFlightAttempt);
        SubscribeLocalEvent<FlightComponent, MobStateChangedEvent>(OnMobStateChangedEvent);
        SubscribeLocalEvent<FlightComponent, EntityZombifiedEvent>(OnFlightDisablingEvent);
        SubscribeLocalEvent<FlightComponent, KnockedDownEvent>(OnFlightDisablingEvent);
        SubscribeLocalEvent<FlightComponent, StunnedEvent>(OnFlightDisablingEvent);
        SubscribeLocalEvent<FlightComponent, DownedEvent>(OnDowned);
        SubscribeLocalEvent<FlightComponent, SleepStateChangedEvent>(OnSleep);
        SubscribeLocalEvent<FlightComponent, AttemptClimbEvent>(OnAttemptClimb);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FlightComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (!component.On)
                continue;

            component.TimeUntilFlap -= frameTime;

            if (component.TimeUntilFlap <= 0f)
            {
                _audio.PlayPredicted(component.FlapSound, uid, uid);
                component.TimeUntilFlap = component.FlapInterval;
            }

            _staminaSystem.TakeStaminaDamage(uid, component.StaminaDrainRate * frameTime, visual: false);
        }
    }

    #region Core Functions
    private void OnStartup(EntityUid uid, FlightComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, ref component.ToggleActionEntity, component.ToggleAction);
    }

    private void OnShutdown(EntityUid uid, FlightComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.ToggleActionEntity);
        if (!TerminatingOrDeleted(uid))
            ToggleActive(uid, false, component);
    }

    public void ToggleActive(EntityUid uid, bool active, FlightComponent component, bool gracefulStop = true)
    {
        component.On = active;
        component.TimeUntilFlap = 0f;
        _actionsSystem.SetToggled(component.ToggleActionEntity, component.On);

        _gravity.RefreshWeightless(uid);

        _movementSpeed.RefreshWeightlessModifiers(uid);
        ToggleCollisionMasks(uid, component);
        UpdateHands(uid, active);

        if (component.CanFail && !gracefulStop)
            _damageable.TryChangeDamage(uid, component.FailDamageSpecifier);

        Dirty(uid, component);
    }

    private void OnToggleFlight(EntityUid uid, FlightComponent component, ToggleFlightEvent args)
    {
        if (!component.On && !CanFly(uid))
            return;

        ToggleActive(uid, !component.On, component);
    }

    private void ToggleCollisionMasks(EntityUid uid, FlightComponent component)
    {
        if (component.On)
            DisableCollisionMasks(uid, component);
        else
            EnableCollisionMasks(uid, component);
    }

    private void DisableCollisionMasks(EntityUid uid, FlightComponent component)
    {
        if (!component.ChangeCollisionMasks
            || !TryComp(uid, out FixturesComponent? fixtureComponent))
            return;

        foreach (var (key, fixture) in fixtureComponent.Fixtures)
        {
            var newMask = (fixture.CollisionMask
                & (int) ~CollisionGroup.HighImpassable
                & (int) ~CollisionGroup.MidImpassable)
                | (int) CollisionGroup.InteractImpassable;

            if (fixture.CollisionMask == newMask)
                continue;

            component.ChangedFixtures.Add((key, fixture.CollisionMask));
            _physics.SetCollisionMask(uid,
                key,
                fixture,
                newMask,
                manager: fixtureComponent);
        }
    }

    private void EnableCollisionMasks(EntityUid uid, FlightComponent component)
    {
        if (!component.ChangeCollisionMasks)
            return;

        if (TryComp(uid, out FixturesComponent? fixtureComponent))
            foreach (var (key, originalMask) in component.ChangedFixtures)
                if (fixtureComponent.Fixtures.TryGetValue(key, out var fixture))
                    _physics.SetCollisionMask(uid, key, fixture, UndoFlightMask(fixture.CollisionMask, originalMask), fixtureComponent);

        component.ChangedFixtures.Clear();
    }

    private static int UndoFlightMask(int currentMask, int originalMask)
    {
        var restored = currentMask
            | (originalMask & (int) (CollisionGroup.HighImpassable | CollisionGroup.MidImpassable));

        if ((originalMask & (int) CollisionGroup.InteractImpassable) == 0)
            restored &= (int) ~CollisionGroup.InteractImpassable;

        return restored;
    }

    private void UpdateHands(EntityUid uid, bool flying)
    {
        if (!TryComp<HandsComponent>(uid, out var handsComponent))
            return;

        if (flying)
            BlockHands(uid, handsComponent);
        else
            FreeHands(uid);
    }

    private void BlockHands(EntityUid uid, HandsComponent handsComponent)
    {
        var freeHands = 0;
        foreach (var hand in _hands.EnumerateHands((uid, handsComponent)))
        {
            if (!_hands.TryGetHeldItem((uid, handsComponent), hand, out var held))
            {
                freeHands++;
                continue;
            }

            if (HasComp<UnremoveableComponent>(held) && held != uid)
                continue;

            _hands.DoDrop((uid, handsComponent), hand);
            freeHands++;
            if (freeHands == 2)
                break;
        }
        if (_virtualItem.TrySpawnVirtualItemInHand(uid, uid, out var virtItem1))
            EnsureComp<UnremoveableComponent>(virtItem1.Value);

        if (_virtualItem.TrySpawnVirtualItemInHand(uid, uid, out var virtItem2))
            EnsureComp<UnremoveableComponent>(virtItem2.Value);
    }

    private void FreeHands(EntityUid uid) => _virtualItem.DeleteInHandsMatching(uid, uid);

    private void OnRefreshWeightlessMoveSpeed(EntityUid uid, FlightComponent component, ref RefreshWeightlessModifiersEvent args)
    {
        if (!component.On)
            return;

        args.ModifyAcceleration(component.SpeedModifier);
        args.ModifyFriction(component.FrictionModifier, component.FrictionNoInputModifier);
    }

    private void OnBeforeStaminaDamage(EntityUid uid, FlightComponent component, ref BeforeStaminaDamageEvent args)
    {
        if (!component.On || args.Value > 0)
            return;

        args.Value *= component.StaminaRegenMultiplier;
    }

    #endregion

    #region Conditionals

    private bool CanFly(EntityUid uid)
    {
        var ev = new FlightAttemptEvent();
        RaiseLocalEvent(uid, ref ev);

        return !ev.Cancelled;
    }

    private void OnCuffableFlightAttempt(EntityUid uid, CuffableComponent component, ref FlightAttemptEvent args)
    {
        if (component.CanStillInteract)
            return;

        _popupSystem.PopupClient(Loc.GetString("no-flight-while-restrained"), uid, uid, PopupType.Medium);
        args.Cancel();
    }

    private void OnZombieFlightAttempt(EntityUid uid, ZombieComponent component, ref FlightAttemptEvent args)
    {
        _popupSystem.PopupClient(Loc.GetString("no-flight-while-zombified"), uid, uid, PopupType.Medium);
        args.Cancel();
    }

    private void OnStandingStateFlightAttempt(EntityUid uid, StandingStateComponent component, ref FlightAttemptEvent args)
    {
        if (!_standing.IsDown(uid))
            return;

        _popupSystem.PopupClient(Loc.GetString("no-flight-while-lying"), uid, uid, PopupType.Medium);
        args.Cancel();
    }

    #endregion

    #region Misc Handlers
    private void OnMobStateChangedEvent(EntityUid uid, FlightComponent component, ref MobStateChangedEvent args)
    {
        if (!component.On || args.NewMobState is not (MobState.Critical or MobState.Dead))
            return;

        ToggleActive(args.Target, false, component, gracefulStop: false);
    }

    private void OnSleep(EntityUid uid, FlightComponent component, ref SleepStateChangedEvent args)
    {
        if (!component.On || !args.FellAsleep)
            return;

        ToggleActive(uid, false, component, gracefulStop: false);
    }

    private void OnDowned(EntityUid uid, FlightComponent component, ref DownedEvent args)
    {
        if (!component.On)
            return;

        ToggleActive(uid, false, component, gracefulStop: false);
    }

    private void OnFlightDisablingEvent<T>(EntityUid uid, FlightComponent component, ref T args) where T : notnull
    {
        if (!component.On)
            return;

        ToggleActive(uid, false, component, gracefulStop: false);
    }

    private void OnAttemptClimb(EntityUid uid, FlightComponent component, ref AttemptClimbEvent args)
    {
        if (!component.On)
            return;

        args.Cancelled = true;
    }

    #endregion
}
