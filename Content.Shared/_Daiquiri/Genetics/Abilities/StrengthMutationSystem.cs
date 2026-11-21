// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class StrengthMutationSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private void OnGetMeleeDamage(Entity<StrengthMutationComponent> ent, ref GetMeleeDamageEvent args)
    {
        args.Damage *= ent.Comp.MeleeModifier;
    }
}
