using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.GameTicking.Prototypes;

/// <summary>
/// Prototype for a lobby music playlist. ID must match the audio file path.
/// </summary>
[Prototype]
public sealed partial class LobbyPlaylistPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; set; } = default!;

    /// <summary>
    /// Display name of the playlist.
    /// </summary>
    [DataField]
    public string Name = "Unknown Playlist";

    /// <summary>
    /// Track file paths in this playlist.
    /// </summary>
    [DataField]
    public List<ResPath> Tracks = new();
}
