// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.EntityEffects;
using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

[RegisterComponent, NetworkedComponent, Access(typeof(EffectActionSystem))]
public sealed partial class EffectActionComponent : Component
{
    [DataField(required: true)]
    public EntityEffect[] Effects = default!;

    [DataField]
    public bool OnPerformed;
}

public sealed partial class EffectInstantActionEvent : InstantActionEvent;

public sealed partial class EffectTargetActionEvent : EntityTargetActionEvent;
