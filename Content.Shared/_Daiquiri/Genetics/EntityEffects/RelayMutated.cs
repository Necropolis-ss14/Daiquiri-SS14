// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects;
using Content.Shared._Daiquiri.Genetics.Mutations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityEffects;

public sealed partial class RelayMutated : EntityEffectBase<RelayMutated>
{
    [DataField(required: true)]
    public EntityEffect Effect = default!;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
        => loc.GetString("entity-effect-guidebook-relay-mutated", ("chance", Probability), ("effect", Effect.EntityEffectGuidebookText(prototype, entSys, loc) ?? string.Empty));
}

public sealed partial class RelayMutatedEffectSystem : EntityEffectSystem<MutationComponent, RelayMutated>
{
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    protected override void Effect(Entity<MutationComponent> ent, ref EntityEffectEvent<RelayMutated> args)
    {
        if (ent.Comp.Target is { } mob)
            _effects.TryApplyEffect(mob, args.Effect.Effect, args.Scale, args.User);
    }
}
