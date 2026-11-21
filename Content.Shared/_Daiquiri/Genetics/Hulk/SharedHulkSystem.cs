using Content.Shared.Damage;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Slippery;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.Hulk;

public abstract partial class SharedHulkSystem : EntitySystem
{
    public static readonly EntProtoId StatusEffectStunned = "StatusEffectStunned";
    public static readonly ProtoId<DamageTypePrototype> Structural = "Structural";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HulkComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<HulkComponent, BeforeStaminaDamageEvent>(OnBeforeStaminaDamage);
        SubscribeLocalEvent<HulkComponent, BeforeStatusEffectAddedEvent>(OnBeforeStatusEffect);
        SubscribeLocalEvent<HulkComponent, KnockDownAttemptEvent>(OnKnockDownAttempt);
        SubscribeLocalEvent<HulkComponent, SlipAttemptEvent>(OnSlipAttempt);
        SubscribeLocalEvent<HulkComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnStartup(Entity<HulkComponent> ent, ref ComponentStartup args)
    {
        UpdateColorStartup(ent);
        ent.Comp.StructuralDamage ??= new DamageSpecifier(ProtoMan.Index(Structural), 80f);
    }

    private void OnMeleeHit(Entity<HulkComponent> ent, ref MeleeHitEvent args)
    {
        args.BonusDamage += args.BaseDamage * ent.Comp.FistDamageMultiplier;
        var total = args.BonusDamage.GetTotal();
        if (total > 0 && total > ent.Comp.MaxBonusFistDamage)
            args.BonusDamage *= ent.Comp.MaxBonusFistDamage / total;

        if (ent.Comp.StructuralDamage != null)
            args.BonusDamage += ent.Comp.StructuralDamage;

        if (args.HitEntities.Count > 0)
            Roar(ent, 0.2f);
    }

    private void OnSlipAttempt(Entity<HulkComponent> ent, ref SlipAttemptEvent args)
    {
        args.NoSlip = true;
    }

    private void OnBeforeStatusEffect(Entity<HulkComponent> ent, ref BeforeStatusEffectAddedEvent args)
    {
        if (args.Effect != StatusEffectStunned)
            return;

        Roar(ent);
        args.Cancelled = true;
    }

    private void OnKnockDownAttempt(Entity<HulkComponent> ent, ref KnockDownAttemptEvent args)
    {
        Roar(ent);
        args.Cancelled = true;
    }

    private void OnBeforeStaminaDamage(Entity<HulkComponent> ent, ref BeforeStaminaDamageEvent args)
    {
        args.Cancelled = true;
    }

    protected virtual void UpdateColorStartup(Entity<HulkComponent> hulk)
    {
    }

    public virtual void Roar(Entity<HulkComponent> hulk, float prob = 1f)
    {
    }
}