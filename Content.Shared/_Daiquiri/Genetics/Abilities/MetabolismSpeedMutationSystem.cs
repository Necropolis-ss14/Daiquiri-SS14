// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared._Starlight.Medical.Body.Components;
using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class MetabolismSpeedMutationSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _body = default!;
    [Dependency] private EntityQuery<MetabolizerComponent> _query = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MetabolismSpeedMutationComponent, MutationAddedEvent>(OnAdded);
        SubscribeLocalEvent<MetabolismSpeedMutationComponent, MutationRemovedEvent>(OnRemoved);
    }

    private void OnAdded(Entity<MetabolismSpeedMutationComponent> ent, ref MutationAddedEvent args)
    {
        Modify(args.Target, ent.Comp.Bonus);
    }

    private void OnRemoved(Entity<MetabolismSpeedMutationComponent> ent, ref MutationRemovedEvent args)
    {
        Modify(args.Target, -ent.Comp.Bonus);
    }

    private void Modify(EntityUid uid, float add)
    {
        if (_query.TryComp(uid, out var mobComp))
        {
            mobComp.UpdateIntervalMultiplier += add;
            Dirty(uid, mobComp);
        }

        if (!_body.TryGetOrgansWithComponent<MetabolizerComponent>(uid, out var organs))
            return;

        foreach (var organ in organs)
        {
            organ.Comp1.UpdateIntervalMultiplier += add;
            Dirty(organ.Owner, organ.Comp1);
        }
    }
}
