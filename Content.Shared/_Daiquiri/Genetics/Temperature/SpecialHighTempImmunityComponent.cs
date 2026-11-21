// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Temperature;

[RegisterComponent, NetworkedComponent]
public sealed partial class SpecialHighTempImmunityComponent : Component
{
    public override bool SessionSpecific => true;
}
