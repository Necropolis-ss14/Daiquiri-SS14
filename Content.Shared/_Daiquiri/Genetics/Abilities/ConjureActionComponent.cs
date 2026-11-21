// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

[RegisterComponent, NetworkedComponent]
public sealed partial class ConjureActionComponent : Component
{
    [DataField(required: true)]
    public EntProtoId Spawn;
}

public sealed partial class ConjureActionEvent : InstantActionEvent;
