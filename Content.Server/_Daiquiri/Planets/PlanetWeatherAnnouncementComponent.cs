using Robust.Shared.Utility;

namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// What this weather says out loud when it rolls in over a planet, and again when it blows over.
///
/// It lives on the weather itself rather than on the planet so that a planet with several kinds of
/// weather can let each one speak for itself: a drizzle is not a downpour, and neither of them is a
/// thunderstorm. A weather without this component falls back to whatever its planet says, which is
/// all a single-weather planet ever needs.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetWeatherAnnouncementComponent : Component
{
    /// <summary>
    /// Said to everyone on the planet as the weather arrives.
    /// </summary>
    [DataField]
    public LocId? StartMessage;

    /// <summary>
    /// Said to them as it leaves.
    /// </summary>
    [DataField]
    public LocId? EndMessage;

    /// <summary>
    /// Colour both lines are written in. Left out, the planet's own colour is used.
    /// </summary>
    [DataField]
    public Color? Color;
}