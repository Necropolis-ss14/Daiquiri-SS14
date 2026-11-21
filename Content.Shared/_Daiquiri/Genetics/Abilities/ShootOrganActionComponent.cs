// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Actions;
using Content.Shared.Polymorph;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

[RegisterComponent, NetworkedComponent]
public sealed partial class ShootOrganActionComponent : Component
{
    [DataField(required: true)]
    public ComponentRegistry Organ = default!;

    [DataField(required: true)]
    public ProtoId<PolymorphPrototype> Polymorph;
}

public sealed partial class ShootOrganActionEvent : WorldTargetActionEvent;
