using Robust.Shared.Prototypes;

namespace Content.Shared.GameTicking.Prototypes;

/// <summary>
/// Prototype for a lobby music track. ID must match the audio file path.
/// </summary>
[Prototype]
public sealed partial class LobbyTrackPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; set; } = default!;

    /// <summary>
    /// The title of the track to be displayed in the lobby.
    /// </summary>
    [DataField]
    public string Title = "Unknown Track";

    /// <summary>
    /// The artist who made the track.
    /// </summary>
    [DataField]
    public string Artist = "Unknown Artist";
}
