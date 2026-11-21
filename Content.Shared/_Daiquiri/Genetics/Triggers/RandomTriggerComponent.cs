// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Trigger.Components.Triggers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;

namespace Content.Shared._Daiquiri.Genetics.Triggers;

[RegisterComponent, NetworkedComponent, Access(typeof(RandomTriggerSystem))]
[AutoGenerateComponentPause, AutoGenerateComponentState]
public sealed partial class RandomTriggerComponent : BaseTriggerOnXComponent
{
    [DataField(required: true)]
    public float Prob;

    [DataField]
    public TimeSpan UpdateDelay = TimeSpan.FromSeconds(1);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextUpdate = TimeSpan.Zero;
}
