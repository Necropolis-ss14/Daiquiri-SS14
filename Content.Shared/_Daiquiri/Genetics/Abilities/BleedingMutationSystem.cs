// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Body.Components;
using Content.Shared._Starlight.Medical.Body.Events;
using Content.Shared._Starlight.Medical.Body.Systems;
using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class BleedingMutationSystem : EntitySystem
{
    [Dependency] private SharedBloodstreamSystem _bloodstream = default!;
    [Dependency] private EntityQuery<BloodstreamComponent> _bloodstreamQuery = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BleedingMutationComponent, MutationAddedEvent>(OnAdded);
        SubscribeLocalEvent<BleedingMutationComponent, MutationRemovedEvent>(OnRemoved);
        SubscribeLocalEvent<BleedingMutationComponent, BleedModifierEvent>(OnBleedModifier);
    }

    private void OnAdded(Entity<BleedingMutationComponent> ent, ref MutationAddedEvent args)
    {
        if (!_bloodstreamQuery.TryComp(args.Target, out var blood))
            return;

        blood.BloodRefreshAmount *= ent.Comp.RefreshModifier;
        Dirty(args.Target, blood);
    }

    private void OnRemoved(Entity<BleedingMutationComponent> ent, ref MutationRemovedEvent args)
    {
        if (!_bloodstreamQuery.TryComp(args.Target, out var blood))
            return;

        blood.BloodRefreshAmount /= ent.Comp.RefreshModifier;
        Dirty(args.Target, blood);
    }

    private void OnBleedModifier(Entity<BleedingMutationComponent> ent, ref BleedModifierEvent args)
    {
        args.BleedAmount *= ent.Comp.BleedModifier;
    }
}
