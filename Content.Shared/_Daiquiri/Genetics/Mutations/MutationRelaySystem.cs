// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Damage.Systems;
using Content.Shared.Flash;
using Content.Shared.Mobs;
using Content.Shared.Speech;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared._Starlight.Medical.Body.Events;

namespace Content.Shared._Daiquiri.Genetics.Mutations;

/// <summary>
/// Relays some events from the mutated mob to the mutation entities.
/// </summary>
public sealed class MutationRelaySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MutatableComponent, AfterFlashedEvent>(RelayEvent);
        SubscribeLocalEvent<MutatableComponent, MobStateChangedEvent>(RelayEvent);
        SubscribeLocalEvent<MutatableComponent, BleedModifierEvent>(RelayEvent);
        SubscribeLocalEvent<MutatableComponent, DamageModifyEvent>(RelayEvent);
        SubscribeLocalEvent<MutatableComponent, GetMeleeDamageEvent>(RelayEvent);
        SubscribeLocalEvent<MutatableComponent, AccentGetEvent>(RelayEvent);
    }

    public void RelayEvent<T>(Entity<MutatableComponent> ent, ref T args) where T: notnull
    {
        foreach (var uid in ent.Comp.Mutations.Values)
        {
            RaiseLocalEvent(uid, ref args);
        }
    }
}
