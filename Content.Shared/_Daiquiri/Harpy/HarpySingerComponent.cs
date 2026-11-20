using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Harpy;

/// <summary>
///     Allows a harpy to sing with their natural voice by opening the MIDI interface.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HarpySingerComponent : Component
{
    [DataField(serverOnly: true)]
    public EntProtoId? MidiActionId = "ActionHarpyPlayMidi";

    [DataField(serverOnly: true)]
    public EntityUid? MidiAction;

    /// <summary>
    ///     Total damage that interrupts singing.
    /// </summary>
    [DataField(serverOnly: true)]
    public int? ShutUpDamageThreshold;
}
