// SPDX-License-Identifier: AGPL-3.0-or-later


using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
namespace Content.Shared._Daiquiri.Genetics.Mutations;

/// <summary>
/// Complete mutation data for a mob which can easily be applied to a mob.
/// </summary>
[DataRecord, Serializable, NetSerializable]
public partial record struct MutatableData(List<EntProtoId> Dormant, List<EntProtoId> Mutations);
