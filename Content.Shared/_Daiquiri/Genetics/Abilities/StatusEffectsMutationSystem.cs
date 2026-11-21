// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.StatusEffectNew;

using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class StatusEffectsMutationSystem : EntitySystem
{
    [Dependency] private StatusEffectsSystem _status = default!;

    [SubscribeLocalEvent]
    private void OnAdded(Entity<StatusEffectsMutationComponent> ent, ref MutationAddedEvent args)
    {
        foreach (var id in ent.Comp.StatusEffects)
        {
            _status.TrySetStatusEffectDuration(args.Target, id);
        }
    }

    [SubscribeLocalEvent]
    private void OnRemoved(Entity<StatusEffectsMutationComponent> ent, ref MutationRemovedEvent args)
    {
        foreach (var id in ent.Comp.StatusEffects)
        {
            _status.TryRemoveStatusEffect(args.Target, id);
        }
    }
}
