// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Actions;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

[RegisterComponent, NetworkedComponent]
public sealed partial class AutotomyMutationComponent : Component;

public sealed partial class AutotomyActionEvent : InstantActionEvent;
