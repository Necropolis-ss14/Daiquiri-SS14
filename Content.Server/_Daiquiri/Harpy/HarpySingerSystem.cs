using Content.Server.Instruments;
using Content.Server.Speech.Components;
using Content.Shared.ActionBlocker;
using Content.Shared._Daiquiri.Harpy;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Instruments;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.UserInterface;
using Content.Shared.Zombies;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.Harpy;

public sealed partial class HarpySingerSystem : EntitySystem
{
    [Dependency] private InstrumentSystem _instrument = default!;
    [Dependency] private SharedPopupSystem _popupSystem = default!;
    [Dependency] private InventorySystem _inventorySystem = default!;
    [Dependency] private ActionBlockerSystem _blocker = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InstrumentComponent, MobStateChangedEvent>(OnMobStateChangedEvent);
        SubscribeLocalEvent<GotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<EntityZombifiedEvent>(OnZombified);
        SubscribeLocalEvent<InstrumentComponent, StunnedEvent>(OnStunned);
        SubscribeLocalEvent<InstrumentComponent, SleepStateChangedEvent>(OnSleep);
        SubscribeLocalEvent<InstrumentComponent, StatusEffectAddedEvent>(OnStatusEffect);
        SubscribeLocalEvent<HarpySingerComponent, BeforeDamageChangedEvent>(OnBeforeDamageChanged);
        SubscribeLocalEvent<HarpySingerComponent, BoundUIClosedEvent>(OnBoundUIClosed);
        SubscribeLocalEvent<HarpySingerComponent, BoundUIOpenedEvent>(OnBoundUIOpened);

        // Intercept the UI action and stop the MIDI UI from opening if the singer is unable to sing.
        SubscribeLocalEvent<HarpySingerComponent, OpenUiActionEvent>(OnInstrumentOpen, before: new[] { typeof(ActivatableUISystem) });
    }

    private void OnEquip(GotEquippedEvent args)
    {
        if (TryComp<AddAccentClothingComponent>(args.Equipment, out var accent) &&
            (accent.ReplacementPrototype?.Id == "mumble" || accent.Accent == "MumbleAccent") &&
            args.Slot == "mask")
        {
            CloseMidiUi(args.EquipTarget);
        }
    }

    private void OnMobStateChangedEvent(EntityUid uid, InstrumentComponent component, ref MobStateChangedEvent args)
    {
        if (args.NewMobState is MobState.Critical or MobState.Dead)
            CloseMidiUi(args.Target);
    }

    private void OnZombified(ref EntityZombifiedEvent args)
    {
        CloseMidiUi(args.Target);
    }

    private void OnStunned(EntityUid uid, InstrumentComponent component, ref StunnedEvent args)
    {
        CloseMidiUi(uid);
    }

    private void OnSleep(EntityUid uid, InstrumentComponent component, ref SleepStateChangedEvent args)
    {
        if (args.FellAsleep)
            CloseMidiUi(uid);
    }

    private void OnStatusEffect(EntityUid uid, InstrumentComponent component, StatusEffectAddedEvent args)
    {
        if (args.Key == "Muted")
            CloseMidiUi(uid);
    }

    private void OnBeforeDamageChanged(EntityUid uid, HarpySingerComponent component, ref BeforeDamageChangedEvent args)
    {
        if (!component.ShutUpDamageThreshold.HasValue || !args.Damage.AnyPositive())
            return;

        var totalApplicableDamage = FixedPoint2.Zero;
        foreach (var (_, value) in args.Damage.GetDamagePerGroup(_prototype))
        {
            totalApplicableDamage += value;
        }

        if (totalApplicableDamage >= component.ShutUpDamageThreshold)
            CloseMidiUi(uid);
    }

    private void CloseMidiUi(EntityUid uid)
    {
        if (!HasComp<ActiveInstrumentComponent>(uid) || !TryComp<ActorComponent>(uid, out var actor))
            return;

        var ent = actor.PlayerSession.AttachedEntity;
        if (ent == null)
            return;

        _instrument.ToggleInstrumentUi(uid, ent.Value);
    }

    private void OnInstrumentOpen(EntityUid uid, HarpySingerComponent component, OpenUiActionEvent args)
    {
        var canNotSpeak = !_blocker.CanSpeak(uid);
        var zombified = HasComp<ZombieComponent>(uid);
        var muzzled = _inventorySystem.TryGetSlotEntity(uid, "mask", out var maskUid) &&
            TryComp<AddAccentClothingComponent>(maskUid, out var accent) &&
            (accent.ReplacementPrototype?.Id == "mumble" || accent.Accent == "MumbleAccent");

        args.Handled = canNotSpeak || muzzled || zombified;

        if (args.Handled)
            _popupSystem.PopupEntity(Loc.GetString("no-sing-while-no-speak"), uid, uid, PopupType.Medium);
    }

    private void OnBoundUIClosed(EntityUid uid, HarpySingerComponent component, BoundUIClosedEvent args)
    {
        if (args.UiKey is not InstrumentUiKey)
            return;

        _appearance.SetData(uid, HarpyVisualLayers.Singing, SingingVisualLayer.False);
    }

    private void OnBoundUIOpened(EntityUid uid, HarpySingerComponent component, BoundUIOpenedEvent args)
    {
        if (args.UiKey is not InstrumentUiKey)
            return;

        _appearance.SetData(uid, HarpyVisualLayers.Singing, SingingVisualLayer.True);
    }
}
