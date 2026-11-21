// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Genetics.Console;

[ByRefEvent]
public readonly record struct ScannerConnectedEvent(EntityUid Scanner);

[ByRefEvent]
public readonly record struct ScannerDisconnectedEvent(EntityUid Scanner);

[ByRefEvent]
public readonly record struct ScannerInsertedEvent(EntityUid Scanner, EntityUid Target);

[ByRefEvent]
public readonly record struct ScannerEjectedEvent(EntityUid Scanner, EntityUid Target);
