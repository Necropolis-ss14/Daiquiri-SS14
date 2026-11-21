// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Triggers;

[RegisterComponent, NetworkedComponent, Access(typeof(MutationActionOnTriggerSystem))]
[AutoGenerateComponentState]
public sealed partial class MutationActionOnTriggerComponent : BaseXOnTriggerComponent;
