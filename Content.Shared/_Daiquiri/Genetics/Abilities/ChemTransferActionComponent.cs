// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Actions;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

/// <summary>
/// Component for the chemspike transfer action.
/// Removes itself and transfers the user's bloodstream chemicals to the mob the spike is embedded in.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ChemTransferActionComponent : Component;

public sealed partial class ChemTransferActionEvent : InstantActionEvent;
