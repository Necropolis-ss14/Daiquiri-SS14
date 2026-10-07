using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Events;

/// <summary>
/// Client requests a track to be added to or removed from a lobby playlist.
/// </summary>
[Serializable, NetSerializable]
public sealed class LobbyTrackMembershipEvent : EntityEventArgs
{
    public LobbyTrackMembershipEvent(string playlistId, string trackPath, bool add)
    {
        PlaylistId = playlistId;
        TrackPath = trackPath;
        Add = add;
    }

    public string PlaylistId;
    public string TrackPath;
    public bool Add;
}
