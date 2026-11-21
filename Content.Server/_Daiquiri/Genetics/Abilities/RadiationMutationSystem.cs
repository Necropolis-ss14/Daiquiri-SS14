// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Server.Radiation.Systems;
using Content.Shared.Mobs;
using Content.Shared._Daiquiri.Genetics.Abilities;

namespace Content.Server._Daiquiri.Genetics.Abilities;

// fucking chud shit is in server for no reason
public sealed partial class RadiationMutationSystem : EntitySystem
{
    [Dependency] private RadiationSystem _radiation = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RadiationMutationComponent, MobStateChangedEvent>(OnStateChanged);
    }

    private void OnStateChanged(Entity<RadiationMutationComponent> ent, ref MobStateChangedEvent args)
    {
        _radiation.SetSourceEnabled(ent.Owner, args.NewMobState != MobState.Dead);
    }
}
