// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Trigger.Components.Triggers;
using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Triggers;

[RegisterComponent, NetworkedComponent, Access(typeof(TriggerOnFlashedSystem))]
[AutoGenerateComponentState]
public sealed partial class TriggerOnFlashedComponent : BaseTriggerOnXComponent
{
    [DataField]
    public float Prob = 1f;
}
