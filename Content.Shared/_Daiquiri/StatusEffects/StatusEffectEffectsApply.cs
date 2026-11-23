using Content.Shared.EntityEffects;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;

namespace Content.Shared._Daiquiri.StatusEffects;

[RegisterComponent, NetworkedComponent]
public sealed partial class StatusEffectEffectsApplyComponent : Component
{
    [DataField]
    public EntityEffect[]? EffectsOnApply;

    [DataField]
    public EntityEffect[]? EffectsOnRemoval;
}

public sealed partial class StatusEffectEffectsApplySystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusEffectEffectsApplyComponent, StatusEffectAppliedEvent>(OnApplied);
        SubscribeLocalEvent<StatusEffectEffectsApplyComponent, StatusEffectRemovedEvent>(OnRemoval);
    }

    private void OnApplied(Entity<StatusEffectEffectsApplyComponent> ent, ref StatusEffectAppliedEvent args)
    {
        if (ent.Comp.EffectsOnApply is not { } effects || _timing.ApplyingState)
            return;

        _effects.ApplyEffects(args.Target, effects);
    }

    private void OnRemoval(Entity<StatusEffectEffectsApplyComponent> ent, ref StatusEffectRemovedEvent args)
    {
        if (ent.Comp.EffectsOnRemoval is not { } effects
            || _timing.ApplyingState
            || TerminatingOrDeleted(args.Target))
            return;

        _effects.ApplyEffects(args.Target, effects);
    }
}
