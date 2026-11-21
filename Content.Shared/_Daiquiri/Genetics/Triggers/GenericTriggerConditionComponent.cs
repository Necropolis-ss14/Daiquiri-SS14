// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityConditions;
using Content.Shared.Trigger.Components.Conditions;
using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Triggers;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GenericTriggerConditionComponent : BaseTriggerConditionComponent
{
    [DataField(required: true)]
    public EntityCondition Condition = default!;

    [DataField]
    public bool CheckUser;
}
