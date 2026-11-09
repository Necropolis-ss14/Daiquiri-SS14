using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Planets;

/// <summary>
/// Interface key of the planet console.
/// </summary>
[Serializable, NetSerializable]
public enum PlanetConsoleUiKey : byte
{
    Key,
}

/// <summary>
/// Everything the console window needs to draw itself: which planets it can reach and whether
/// a disk is currently being printed.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlanetConsoleState : BoundUserInterfaceState
{
    /// <summary>
    /// Ids of the planets this console offers, already in display order.
    /// </summary>
    public List<string> Planets;

    /// <summary>
    /// Id of the planet whose disk is being printed, or an empty string when the console is idle.
    /// </summary>
    public string Printing;

    /// <summary>
    /// How long a print takes in total, in seconds.
    /// </summary>
    public float PrintDuration;

    /// <summary>
    /// How much of the current print is left, in seconds.
    /// </summary>
    public float PrintRemaining;

    /// <summary>
    /// How long the console still has to recharge before it can print another disk, in seconds.
    /// Zero when a disk can be printed right away.
    /// </summary>
    public float CooldownRemaining;

    /// <summary>
    /// Length of the whole recharge between two jumps, in seconds.
    /// </summary>
    public float CooldownDuration;

    /// <summary>
    /// Localisation id explaining why the console cannot jump at all right now, or an empty string
    /// when it is good to go. The console has to be mounted on a shuttle rather than a station.
    /// </summary>
    public string BlockReason;

    public PlanetConsoleState(List<string> planets, string printing, float printDuration, float printRemaining,
        float cooldownRemaining, float cooldownDuration, string blockReason)
    {
        Planets = planets;
        Printing = printing;
        PrintDuration = printDuration;
        PrintRemaining = printRemaining;
        CooldownRemaining = cooldownRemaining;
        CooldownDuration = cooldownDuration;
        BlockReason = blockReason;
    }
}

/// <summary>
/// Asks the console to chart a course to a planet and jump its grid there.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlanetConsolePrintMessage : BoundUserInterfaceMessage
{
    public string Planet = string.Empty;
}
