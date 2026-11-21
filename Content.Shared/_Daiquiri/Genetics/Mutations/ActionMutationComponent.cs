// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Actions.Components;

namespace Content.Shared._Daiquiri.Genetics.Mutations;

/// <summary>
/// Grants the mutation user an action.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(ActionMutationSystem))]
[AutoGenerateComponentState]
public sealed partial class ActionMutationComponent : Component
{
    [DataField(required: true)]
    public EntProtoId<ActionComponent> Action;

    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;
}
