using Content.Shared.Actions;

namespace Content.Shared._Daiquiri.Harpy;

public sealed partial class ToggleFlightEvent : InstantActionEvent;

[ByRefEvent]
public sealed class FlightAttemptEvent : CancellableEntityEventArgs;
