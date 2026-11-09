using Content.Server.Chat.Managers;
using Content.Server.Electrocution;
using Content.Shared._Daiquiri.Planets;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Light.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Weather;
using Robust.Server.Player;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// Runs the weather on generated planets: a strong snowfall on Taipan, an ashfall on Lavaland and
/// one of several rains on Novo Nihon, all of them arriving every five to ten minutes and blowing
/// over after one to three. A planet that has more than one kind of weather picks at random.
///
/// While a storm is running everyone standing under the open sky takes one point of damage per
/// second - cold on Taipan, heat on Lavaland, nothing at all on Novo Nihon. The weather system's
/// own check decides what counts as shelter, so a grid, a roofed tile or a tile the weather cannot
/// spread over is safe.
///
/// Novo Nihon's storm also drops the occasional bolt on a random tile nearby, which hurts whoever
/// it comes down on - see <see cref="TryStrike"/>.
///
/// The line put over the chat is the weather's own where it has one - see
/// <see cref="PlanetWeatherAnnouncementComponent"/> - which is how Novo Nihon tells its drizzle
/// apart from its downpour. A weather with nothing to say falls back to the planet's wording.
/// </summary>
public sealed partial class PlanetWeatherSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedWeatherSystem _weather = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private DamageableSystem _damage = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private IChatManager _chat = default!;
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private ElectrocutionSystem _electrocution = default!;

    /// <summary>
    /// How long the planet stays clear between two storms.
    /// </summary>
    private static readonly TimeSpan CalmMin = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CalmMax = TimeSpan.FromMinutes(10);

    /// <summary>
    /// How long a storm lasts.
    /// </summary>
    private static readonly TimeSpan StormMin = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan StormMax = TimeSpan.FromMinutes(3);

    /// <summary>
    /// How close to whoever it is aimed at the bolt may land, and how far away. Both in tiles: it
    /// has to be visible to be worth watching, and it should not land on their head every time.
    /// </summary>
    private const float StrikeMinDistance = 3f;
    private const float StrikeMaxDistance = 14f;

    /// <summary>
    /// How close to where the bolt landed something has to be standing to be hit by it.
    /// </summary>
    private const float StrikeHitRadius = 0.9f;

    /// <summary>
    /// How long a bolt leaves whoever it comes down on paralysed, alongside the shock damage.
    /// </summary>
    private static readonly TimeSpan StrikeStunTime = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Puts a planet on the storm schedule. Called once the map is generated and running.
    /// </summary>
    public void StartTracking(EntityUid mapUid, PlanetDestinationPrototype planet)
    {
        var comp = EnsureComp<PlanetWeatherComponent>(mapUid);
        comp.Weathers = new List<EntProtoId>(planet.Weathers);
        comp.Current = null;
        comp.CurrentWeather = null;
        comp.StartMessage = planet.WeatherStartMessage;
        comp.EndMessage = planet.WeatherEndMessage;
        comp.MessageColor = planet.WeatherColor;
        comp.DamageType = planet.WeatherDamageType;
        comp.DamagePerSecond = planet.WeatherDamagePerSecond;
        comp.Lightning = planet.Lightning;
        comp.LightningDuring = planet.LightningDuring;
        comp.LightningChancePerSecond = planet.LightningChancePerSecond;
        comp.LightningShockDamage = planet.LightningShockDamage;
        comp.Active = false;
        comp.StormEnd = TimeSpan.Zero;
        comp.DamageAccumulator = 0f;
        comp.NextStorm = _timing.CurTime + RandomBetween(CalmMin, CalmMax);
    }

    public override void Update(float frameTime)
    {
        var now = _timing.CurTime;
        var query = EntityQueryEnumerator<PlanetWeatherComponent>();

        while (query.MoveNext(out var mapUid, out var comp))
        {
            if (comp.Active && now >= comp.StormEnd)
                EndStorm(mapUid, comp);
            else if (!comp.Active && now >= comp.NextStorm)
                StartStorm(mapUid, comp);

            if (comp.Current is not { } current)
                continue;

            // Both of these follow the weather itself rather than the schedule, so weather started
            // by hand with the weather command hurts and strikes just as much as a scheduled one.
            if (!_weather.HasWeather(Transform(mapUid).MapID, current))
                continue;

            DamageOpenSky(mapUid, comp, frameTime);
            TryStrike(mapUid, comp, frameTime);
        }
    }

    private void StartStorm(EntityUid mapUid, PlanetWeatherComponent comp)
    {
        // A planet with no weather to speak of only ever gets the rest between storms.
        if (comp.Weathers.Count == 0)
        {
            comp.NextStorm = _timing.CurTime + RandomBetween(CalmMin, CalmMax);
            return;
        }

        var duration = RandomBetween(StormMin, StormMax);
        var weather = _random.Pick(comp.Weathers);

        // The map may have gone away between two storms, in which case the component went with it.
        if (!_weather.TryAddWeather(mapUid, weather, out var weatherEnt, duration))
        {
            comp.NextStorm = _timing.CurTime + RandomBetween(CalmMin, CalmMax);
            return;
        }

        comp.Current = weather;
        comp.CurrentWeather = weatherEnt;
        comp.Active = true;
        comp.StormEnd = _timing.CurTime + duration;
        comp.DamageAccumulator = 0f;

        // The weather speaks for itself where it has something to say, which is how a planet with
        // several kinds of weather says which one just arrived.
        var announcement = GetAnnouncement(weatherEnt);
        Announce(mapUid, announcement?.StartMessage ?? comp.StartMessage, announcement?.Color ?? comp.MessageColor);
    }

    private void EndStorm(EntityUid mapUid, PlanetWeatherComponent comp)
    {
        // Read before the weather goes: the words are the weather's own.
        var announcement = GetAnnouncement(comp.CurrentWeather);

        if (comp.Current is { } current)
            _weather.TryRemoveWeather(mapUid, current);

        comp.Current = null;
        comp.CurrentWeather = null;
        comp.Active = false;
        comp.NextStorm = _timing.CurTime + RandomBetween(CalmMin, CalmMax);
        Announce(mapUid, announcement?.EndMessage ?? comp.EndMessage, announcement?.Color ?? comp.MessageColor);
    }

    /// <summary>
    /// What the given weather has to say for itself, or null if it would rather leave the talking to
    /// its planet.
    /// </summary>
    private PlanetWeatherAnnouncementComponent? GetAnnouncement(EntityUid? weather)
    {
        if (weather is not { } uid || TerminatingOrDeleted(uid))
            return null;

        return TryComp<PlanetWeatherAnnouncementComponent>(uid, out var announcement) ? announcement : null;
    }

    /// <summary>
    /// Drops a bolt out of the sky now and then while the planet's storming weather is the one
    /// running. It comes down on a random tile in sight of someone standing out in the open, and
    /// whoever is standing there when it lands takes the shock.
    /// </summary>
    private void TryStrike(EntityUid mapUid, PlanetWeatherComponent comp, float frameTime)
    {
        if (comp.Lightning is not { } bolt || comp.Current != comp.LightningDuring)
            return;

        // A chance per second, rolled per frame, which is the same thing and needs no clock.
        if (comp.LightningChancePerSecond <= 0f ||
            _random.NextFloat() > comp.LightningChancePerSecond * frameTime)
        {
            return;
        }

        if (PickStrikeSpot(mapUid) is not { } spot)
            return;

        Spawn(bolt, spot);
        ShockAround(mapUid, spot, comp.LightningShockDamage);
    }

    /// <summary>
    /// Somewhere near one of the people out in the open, or nothing at all when there is nobody to
    /// aim at. The bolt is nudged a little way off its mark so it lands on a tile of its own
    /// rather than on the head of whoever it was aimed at.
    /// </summary>
    private EntityCoordinates? PickStrikeSpot(EntityUid mapUid)
    {
        var outInTheOpen = new List<EntityCoordinates>();

        foreach (var session in _player.Sessions)
        {
            if (session.AttachedEntity is not { } mob)
                continue;

            var xform = Transform(mob);

            if (xform.MapUid != mapUid || IsSheltered(mapUid, xform))
                continue;

            outInTheOpen.Add(xform.Coordinates);
        }

        if (outInTheOpen.Count == 0)
            return null;

        return _random.Pick(outInTheOpen).Offset(_random.NextVector2(StrikeMinDistance, StrikeMaxDistance));
    }

    /// <summary>
    /// Shocks everything standing where the bolt came down, through the ordinary electrocution, so
    /// it lands the way any other shock does: paralysed, jittering and hurt, insulation and all.
    /// Someone in insulated gloves takes nothing - their siemens coefficient is zero - and budget
    /// gloves take part of it, which is exactly what the game does everywhere else.
    ///
    /// The bolt entity cannot do this by itself: it is spawned without fixtures, so nothing ever
    /// bumps into it and its own electrified component never fires.
    /// </summary>
    private void ShockAround(EntityUid mapUid, EntityCoordinates spot, float amount)
    {
        if (amount <= 0f)
            return;

        var damage = (int) amount;

        var mobs = EntityQueryEnumerator<MobStateComponent, TransformComponent>();

        while (mobs.MoveNext(out var uid, out var state, out var xform))
        {
            if (state.CurrentState == MobState.Dead || xform.MapUid != mapUid || IsSheltered(mapUid, xform))
                continue;

            // Both coordinates hang off the planet's own surface, so the difference is meaningful.
            if ((xform.Coordinates.Position - spot.Position).Length() > StrikeHitRadius)
                continue;

            _electrocution.TryDoElectrocution(uid, null, damage, StrikeStunTime, refresh: false);
        }
    }

    /// <summary>
    /// Hurts everyone standing under the open sky, one point of damage per second. Whoever is
    /// under a roof, on a grid floor or on a tile the weather cannot spread over is left alone.
    /// </summary>
    private void DamageOpenSky(EntityUid mapUid, PlanetWeatherComponent comp, float frameTime)
    {
        if (comp.DamagePerSecond <= 0f)
            return;

        comp.DamageAccumulator += frameTime;

        if (comp.DamageAccumulator < 1f)
            return;

        // Frame times are not whole seconds, so the whole ticks are applied and the remainder is
        // carried over to the next frame.
        var ticks = (int) comp.DamageAccumulator;
        comp.DamageAccumulator -= ticks;

        var damage = new DamageSpecifier(
            _prototype.Index(comp.DamageType),
            FixedPoint2.New(comp.DamagePerSecond * ticks));

        var mobs = EntityQueryEnumerator<MobStateComponent, DamageableComponent, TransformComponent>();

        while (mobs.MoveNext(out var uid, out var state, out var damageable, out var xform))
        {
            if (state.CurrentState == MobState.Dead)
                continue;

            if (xform.MapUid != mapUid || IsSheltered(mapUid, xform))
                continue;

            _damage.TryChangeDamage((uid, damageable), damage);
        }
    }

    /// <summary>
    /// Whether what the entity is standing on keeps the storm off. Anything the weather system
    /// itself considers protected counts, which covers grids, roofs and weather-proof tiles.
    /// </summary>
    private bool IsSheltered(EntityUid mapUid, TransformComponent xform)
    {
        if (xform.GridUid is not { } gridUid)
            return false;

        // Any grid that is not the planet's own surface is a shuttle, a station or something the
        // crew built, and keeps the storm off.
        if (gridUid != mapUid)
            return true;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        if (!_map.TryGetTileRef(gridUid, grid, xform.Coordinates, out var tile))
            return false;

        TryComp<RoofComponent>(gridUid, out var roof);

        return !_weather.CanWeatherAffect(
            new Entity<MapGridComponent?, RoofComponent?>(gridUid, grid, roof),
            tile);
    }

    /// <summary>
    /// Writes one line to everyone standing on the planet, and to nobody else.
    /// </summary>
    private void Announce(EntityUid mapUid, LocId? message, Color color)
    {
        if (message == null)
            return;

        var text = Loc.GetString(message.Value);
        var filter = Filter.Empty();

        foreach (var session in _player.Sessions)
        {
            if (session.AttachedEntity is not { } mob)
                continue;

            if (Transform(mob).MapUid != mapUid)
                continue;

            filter.AddPlayer(session);
        }

        if (filter.Count == 0)
            return;

        _chat.ChatMessageToManyFiltered(filter, ChatChannel.Server, text, text, mapUid, false, false,
            color);
    }

    private TimeSpan RandomBetween(TimeSpan min, TimeSpan max)
    {
        var span = (max - min).TotalSeconds;
        return TimeSpan.FromSeconds(min.TotalSeconds + _random.NextFloat() * span);
    }
}