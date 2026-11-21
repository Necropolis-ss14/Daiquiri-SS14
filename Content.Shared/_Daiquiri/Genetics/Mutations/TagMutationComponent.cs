// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Tag;

namespace Content.Shared._Daiquiri.Genetics.Mutations;

/// <summary>
/// Adds or removes tags to the target mob.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class TagMutationComponent : Component
{
    [DataField]
    public List<ProtoId<TagPrototype>> Added = new();

    [DataField]
    public List<ProtoId<TagPrototype>> Removed = new();
}
