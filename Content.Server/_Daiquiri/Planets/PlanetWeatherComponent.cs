using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// Periodic storm on a generated planet map. The map gets one of these as soon as it is created,
/// and the system behind it starts and stops the storm on its own.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetWeatherComponent : Component
{
    /// <summary>
    /// Weather prototypes the planet draws from, one per storm (visuals, sound and what counts as
    /// shelter all come from the chosen one).
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public List<EntProtoId> Weathers = new();

    /// <summary>
    /// The one running right now, if any. Cleared when the storm breaks.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId? Current;

    /// <summary>
    /// The entity that weather was spawned as. It is the one that knows what the storm has to say
    /// for itself, so it is kept for as long as the storm lasts.
    /// </summary>
    [ViewVariables]
    public EntityUid? CurrentWeather;

    /// <summary>
    /// Bolt dropped on a random tile while <see cref="LightningDuring"/> is the weather running.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId? Lightning;

    /// <summary>
    /// The weather the bolts belong to.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId LightningDuring = string.Empty;

    /// <summary>
    /// Chance of a bolt every second of that weather.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float LightningChancePerSecond;

    /// <summary>
    /// Damage a bolt deals to whoever it lands on.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float LightningShockDamage;

    /// <summary>
    /// Announced to everyone on the map when the storm rolls in.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public LocId? StartMessage;

    /// <summary>
    /// Announced to everyone on the map when the storm breaks.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public LocId? EndMessage;

    /// <summary>
    /// Colour both announcements are written in.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Color MessageColor = Color.White;

    /// <summary>
    /// Damage type the storm deals to whoever is caught out in the open.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<DamageTypePrototype> DamageType = "Cold";

    /// <summary>
    /// How much of <see cref="DamageType"/> the storm deals per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float DamagePerSecond = 1f;

    /// <summary>
    /// Time carried over between two damage ticks of the running storm.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float DamageAccumulator;

    /// <summary>
    /// Whether the storm is currently running.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Active;

    /// <summary>
    /// When the next storm starts, if none is running.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextStorm;

    /// <summary>
    /// When the running storm ends.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan StormEnd;

    /// <summary>
    /// Seconds until the weather turns, and the one knob worth having in View Variables: it is the
    /// wait the planet is currently serving, whichever of the two that is. With no storm running it
    /// is the calm before the next one; with a storm running it is how much longer that storm
    /// lasts. Writing to it moves that moment to the given number of seconds from now, so setting
    /// 30 while it is clear brings the storm in half a minute and setting 30 while it is raining
    /// breaks it up in half a minute.
    ///
    /// The schedule itself is kept as an absolute time - the two fields above - because the rest of
    /// the system compares them against the round clock, so this property is worked out against
    /// that same clock on the way in and on the way out rather than stored.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float SecondsUntilWeather
    {
        get
        {
            var now = IoCManager.Resolve<IGameTiming>().CurTime;
            return (float) ((Active ? StormEnd : NextStorm) - now).TotalSeconds;
        }
        set
        {
            var target = IoCManager.Resolve<IGameTiming>().CurTime + TimeSpan.FromSeconds(value);

            if (Active)
                StormEnd = target;
            else
                NextStorm = target;
        }
    }
}