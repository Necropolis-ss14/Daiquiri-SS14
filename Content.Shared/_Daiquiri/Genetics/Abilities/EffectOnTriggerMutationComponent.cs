// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.EntityEffects;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

/// <summary>
/// Runs entity effects on the mutation target when this mutation is triggered.
/// The mob state can also be filtered.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EffectOnTriggerMutationComponent : Component
{
    /// <summary>
    /// The effects to run on the target.
    /// </summary>
    [DataField(required: true)]
    public List<EntityEffect> Effects = new();
}
