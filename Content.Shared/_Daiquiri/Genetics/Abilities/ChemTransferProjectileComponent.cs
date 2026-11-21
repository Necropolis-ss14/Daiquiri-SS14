// SPDX-License-Identifier: AGPL-3.0-or-later


using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
namespace Content.Shared._Daiquiri.Genetics.Abilities;

/// <summary>
/// Component for a projectile that adds the chemspike transfer action while embedded in a mob.
/// Needs <c>ActionProjectileComponent</c> to be set, and the action's container to be a mutation.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ChemTransferProjectileComponent : Component;
