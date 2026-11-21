// SPDX-License-Identifier: AGPL-3.0-or-later


using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
namespace Content.Shared._Daiquiri.Genetics.Abilities;

/// <summary>
/// Modifies max zoom when added and automatically zooms out.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(MaxZoomMutationSystem))]
public sealed partial class MaxZoomMutationComponent : Component
{
    /// <summary>
    /// What to scale MaxZoom by.
    /// </summary>
    [DataField]
    public float Modifier = 1.25f;
}
