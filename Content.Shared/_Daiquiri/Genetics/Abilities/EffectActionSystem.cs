// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.Actions.Events;
using Content.Shared.EntityEffects;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class EffectActionSystem : EntitySystem
{
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EffectActionComponent, ActionPerformedEvent>(OnActionPerformed);
        SubscribeLocalEvent<EffectActionComponent, EffectInstantActionEvent>(OnInstantAction);
        SubscribeLocalEvent<EffectActionComponent, EffectTargetActionEvent>(OnTargetAction);
    }

    private void OnActionPerformed(Entity<EffectActionComponent> ent, ref ActionPerformedEvent args)
    {
        var user = args.Performer;
        if (ent.Comp.OnPerformed)
            _effects.ApplyEffects(user, ent.Comp.Effects, user: user);
    }

    private void OnInstantAction(Entity<EffectActionComponent> ent, ref EffectInstantActionEvent args)
    {
        var user = args.Performer;
        _effects.ApplyEffects(user, ent.Comp.Effects, user: user);
        args.Handled = true;
    }

    private void OnTargetAction(Entity<EffectActionComponent> ent, ref EffectTargetActionEvent args)
    {
        var user = args.Performer;
        _effects.ApplyEffects(args.Target, ent.Comp.Effects, user: user);
        args.Handled = true;
    }
}
