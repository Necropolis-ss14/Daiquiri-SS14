using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Events;

/// <summary>
/// Client requests a lobby playlist to be enabled/disabled in the playback pool.
/// </summary>
[Serializable, NetSerializable]
public sealed class LobbyPlaylistToggleEvent : EntityEventArgs
{
    public LobbyPlaylistToggleEvent(string playlistId, bool enabled)
    {
        PlaylistId = playlistId;
        Enabled = enabled;
    }

    public string PlaylistId;
    public bool Enabled;
}
