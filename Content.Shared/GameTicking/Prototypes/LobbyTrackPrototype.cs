using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.GameTicking.Prototypes;

/// <summary>
/// Prototype for a lobby music track.
/// </summary>
[Prototype]
public sealed partial class LobbyTrackPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; set; } = default!;

    [DataField(required: true)]
    public ResPath Path = default!;

    /// <summary>
    /// The title of the track to be displayed in the lobby.
    /// </summary>
    [DataField]
    public string Title = "Unknown Track";


}
