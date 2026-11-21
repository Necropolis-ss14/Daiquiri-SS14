// SPDX-License-Identifier: AGPL-3.0-or-later


using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
namespace Content.Shared._Daiquiri.Genetics.Abilities;

/// <summary>
/// Mutation component that increases mob's flat metabolism rate when active.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(MetabolismSpeedMutationSystem))]
public sealed partial class MetabolismSpeedMutationComponent : Component
{
    [DataField(required: true)]
    public float Bonus;
}
