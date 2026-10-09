using Robust.Shared.Configuration;

namespace Content.Shared._Starlight.CCVar;

public sealed partial class StarlightCCVars
{
    /// <summary>
    /// Daiquiri: locally excluded lobby tracks, "playlist|track;"-separated. Local only.
    /// </summary>
    public static readonly CVarDef<string> LobbyQueueRemoved =
        CVarDef.Create("lobby.queue_removed", "", CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Daiquiri: playlists the player ever touched, ";"-separated. Local only.
    /// </summary>
    public static readonly CVarDef<string> LobbyQueueTouched =
        CVarDef.Create("lobby.queue_touched", "", CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Daiquiri: saved local queue order, ";"-separated. Keeps the queue stable across rejoins.
    /// </summary>
    public static readonly CVarDef<string> LobbyQueueOrder =
        CVarDef.Create("lobby.queue_order", "", CVar.CLIENTONLY | CVar.ARCHIVE);
    /// <summary>
    /// Daiquiri: locally added tracks (e.g. from turnon=false playlists), ";"-separated. Local only.
    /// </summary>
    public static readonly CVarDef<string> LobbyQueueAdded =
        CVarDef.Create("lobby.queue_added", "", CVar.CLIENTONLY | CVar.ARCHIVE);
}
