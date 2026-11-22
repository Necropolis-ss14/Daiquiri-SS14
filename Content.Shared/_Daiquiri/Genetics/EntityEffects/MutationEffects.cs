using System.Linq;
using Content.Shared.CombatMode;
using Content.Shared.EntityEffects;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Stunnable;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Trigger.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Random;
using Content.Shared.Throwing;
using Robust.Shared.Physics;

namespace Content.Shared._Daiquiri.Genetics.EntityEffects;

public sealed partial class WeightedRandomEffect : EntityEffectBase<WeightedRandomEffect>
{
    [DataField(required: true)]
    public List<WeightedEffect> Children = new();

}

public sealed partial class WeightedRandomEffectSystem : EntityEffectSystem<TransformComponent, WeightedRandomEffect>
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    protected override void Effect(Entity<TransformComponent> ent, ref EntityEffectEvent<WeightedRandomEffect> args)
    {
        var total = 0f;
        foreach (var child in args.Effect.Children)
        {
            total += child.Weight;
        }

        var roll = _random.NextFloat() * total;
        total = 0f;
        foreach (var child in args.Effect.Children)
        {
            total += child.Weight;
            if (total < roll)
                continue;

            _effects.TryApplyEffect(ent, child.Effect, args.Scale, args.User);
            return;
        }
    }
}

[DataDefinition]
public partial record struct WeightedEffect()
{
    [DataField(required: true)]
    public EntityEffect Effect = default!;

    [DataField]
    public float Weight = 1f;
}

public sealed partial class ThrowRandomly : EntityEffectBase<ThrowRandomly>
{
    [DataField]
    public float Speed = 10f;
}

public sealed partial class ThrowRandomlyEffectSystem : EntityEffectSystem<TransformComponent, ThrowRandomly>
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private ThrowingSystem _throwing = default!;

    protected override void Effect(Entity<TransformComponent> ent, ref EntityEffectEvent<ThrowRandomly> args)
    {
        var direction = _random.NextAngle().ToVec();
        _throwing.TryThrow(ent, direction, baseThrowSpeed: args.Effect.Speed, user: args.User);
    }
}

public sealed partial class AttackSelf : EntityEffectBase<AttackSelf>
{
    [DataField]
    public bool UseHeld = true;
}

public sealed partial class AttackSelfEffectSystem : EntityEffectSystem<CombatModeComponent, AttackSelf>
{
    [Dependency] private SharedCombatModeSystem _combatMode = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedMeleeWeaponSystem _melee = default!;
    [Dependency] private EntityQuery<MeleeWeaponComponent> _query = default!;

    protected override void Effect(Entity<CombatModeComponent> ent, ref EntityEffectEvent<AttackSelf> args)
    {
        var weapon = ent.Owner;
        if (args.Effect.UseHeld)
            weapon = _hands.GetActiveItemOrSelf(ent.Owner);

        if (!_query.TryComp(weapon, out var weaponComp))
            return;

        var wasOn = ent.Comp.IsInCombatMode;
        _combatMode.SetInCombatMode(ent.Owner, true, ent.Comp);
        _melee.AttemptLightAttack(ent.Owner, weapon, weaponComp, ent.Owner);
        _combatMode.SetInCombatMode(ent.Owner, wasOn, ent.Comp);
    }
}

public sealed partial class DropItems : EntityEffectBase<DropItems>;

public sealed partial class DropItemsEffectSystem : EntityEffectSystem<HandsComponent, DropItems>
{
    [Dependency] private SharedHandsSystem _hands = default!;

    protected override void Effect(Entity<HandsComponent> ent, ref EntityEffectEvent<DropItems> args)
    {
        foreach (var held in _hands.EnumerateHeld(ent.AsNullable()).ToList())
        {
            _hands.TryDrop(ent.AsNullable(), held);
        }
    }
}

public sealed partial class DropRandomItem : EntityEffectBase<DropRandomItem>
{
    [DataField]
    public EntityEffect[] Effects = [];
}

public sealed partial class DropRandomItemEffectSystem : EntityEffectSystem<HandsComponent, DropRandomItem>
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;
    [Dependency] private SharedHandsSystem _hands = default!;

    protected override void Effect(Entity<HandsComponent> ent, ref EntityEffectEvent<DropRandomItem> args)
    {
        var items = _hands.EnumerateHeld(ent.AsNullable()).ToList();
        if (items.Count == 0)
            return;

        var item = _random.Pick(items);
        if (!_hands.TryDrop(ent.AsNullable(), item))
            return;

        _effects.ApplyEffects(item, args.Effect.Effects, args.Scale, args.User ?? ent.Owner);
    }
}

public sealed partial class StartUseDelay : EntityEffectBase<StartUseDelay>
{
    [DataField]
    public string DelayId = UseDelaySystem.DefaultId;
}

public sealed partial class StartUseDelayEffectSystem : EntityEffectSystem<UseDelayComponent, StartUseDelay>
{
    [Dependency] private UseDelaySystem _useDelay = default!;

    protected override void Effect(Entity<UseDelayComponent> ent, ref EntityEffectEvent<StartUseDelay> args)
    {
        _useDelay.TryResetDelay((ent.Owner, ent.Comp), id: args.Effect.DelayId);
    }
}

public sealed partial class PlaySoundEffect : EntityEffectBase<PlaySoundEffect>
{
    [DataField(required: true)]
    public SoundSpecifier Sound = default!;

    [DataField]
    public bool Positional = true;
}

public sealed partial class PlaySoundEffectSystem : EntityEffectSystem<TransformComponent, PlaySoundEffect>
{
    [Dependency] private SharedAudioSystem _audio = default!;

    protected override void Effect(Entity<TransformComponent> ent, ref EntityEffectEvent<PlaySoundEffect> args)
    {
        if (args.Effect.Positional)
            _audio.PlayPvs(args.Effect.Sound, ent.Comp.Coordinates);
        else
            _audio.PlayPvs(args.Effect.Sound, ent);
    }
}

public sealed partial class Knockdown : EntityEffectBase<Knockdown>
{
    [DataField]
    public float Time = 3f;
}

public sealed partial class KnockdownEffectSystem : EntityEffectSystem<CrawlerComponent, Knockdown>
{
    [Dependency] private SharedStunSystem _stun = default!;

    protected override void Effect(Entity<CrawlerComponent> ent, ref EntityEffectEvent<Knockdown> args)
    {
        _stun.TryKnockdown(ent.Owner, TimeSpan.FromSeconds(args.Effect.Time));
    }
}

[RegisterComponent, NetworkedComponent]
public sealed partial class TriggerOnWalkComponent : Component
{
    [DataField]
    public string KeyOut = "walk";
}

public sealed partial class TriggerOnWalkSystem : EntitySystem
{
    [Dependency] private TriggerSystem _trigger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnWalkComponent, GetFootstepSoundEvent>(OnFootstep);
    }

    private void OnFootstep(Entity<TriggerOnWalkComponent> ent, ref GetFootstepSoundEvent args)
    {
        _trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
    }
}

public sealed partial class AttackOthers : EntityEffectBase<AttackOthers>
{
    [DataField]
    public float Range = 1.5f;

    [DataField]
    public bool UseHeld = true;
}

public sealed partial class AttackOthersEffectSystem : EntityEffectSystem<CombatModeComponent, AttackOthers>
{
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedCombatModeSystem _combatMode = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedMeleeWeaponSystem _melee = default!;
    [Dependency] private EntityQuery<MeleeWeaponComponent> _query = default!;

    protected override void Effect(Entity<CombatModeComponent> ent, ref EntityEffectEvent<AttackOthers> args)
    {
        var targets = new List<EntityUid>();
        foreach (var other in _lookup.GetEntitiesInRange(ent.Owner, args.Effect.Range))
        {
            if (other != ent.Owner && HasComp<MobStateComponent>(other))
                targets.Add(other);
        }

        if (targets.Count == 0)
            return;

        var target = _random.Pick(targets);
        var weapon = args.Effect.UseHeld ? _hands.GetActiveItemOrSelf(ent.Owner) : ent.Owner;
        if (!_query.TryComp(weapon, out var weaponComp))
            return;

        var wasOn = ent.Comp.IsInCombatMode;
        _combatMode.SetInCombatMode(ent.Owner, true, ent.Comp);
        _melee.AttemptLightAttack(ent.Owner, weapon, weaponComp, target);
        _combatMode.SetInCombatMode(ent.Owner, wasOn, ent.Comp);
    }
}
