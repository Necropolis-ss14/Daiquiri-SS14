using System.Linq;
using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Jittering;
using Content.Server.Parallax;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared._Daiquiri.Planets;
using Content.Shared.Atmos;
using Content.Shared.Light.Components;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// Charts a course to a planet and jumps the grid the console is bolted to there. Only works off a
/// station: the console refuses to send the station itself anywhere.
/// </summary>
public sealed partial class PlanetConsoleSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private AtmosphereSystem _atmos = default!;
    [Dependency] private BiomeSystem _biome = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private ShuttleSystem _shuttle = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private JitteringSystem _jitter = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private PlanetWeatherSystem _planetWeather = default!;

    /// <summary>
    /// Work log the console calls out while it charts a course, one line per step.
    /// </summary>
    private static readonly LocId[] PrintSteps =
    {
        "planet-console-step-routing",
        "planet-console-step-calibration",
        "planet-console-step-safe-path",
        "planet-console-step-coordinates",
        "planet-console-step-beacon",
        "planet-console-step-recording",
    };

    /// <summary>
    /// How hard the console rattles while it works. The engine caps the visible offset, so this is
    /// about as rough as a shake gets.
    /// </summary>
    private const float PrintShakeAmplitude = 80f;
    private const float PrintShakeFrequency = 8f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlanetConsoleComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<PlanetConsoleComponent, PlanetConsolePrintMessage>(OnPrintMessage);
    }

    private void OnUiOpened(Entity<PlanetConsoleComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnPrintMessage(Entity<PlanetConsoleComponent> ent, ref PlanetConsolePrintMessage args)
    {
        if (ent.Comp.Printing != null)
            return;

        // A console on a station would take the whole station with it, so it never starts.
        if (GetBlockReason(ent.Owner) is { } blocked)
        {
            _popup.PopupEntity(Loc.GetString(blocked), ent.Owner);
            UpdateUi(ent);
            return;
        }

        // The recharge lives on the grid, not on the console, so rebuilding the console does not
        // reset it.
        if (Transform(ent.Owner).GridUid is not { } gridUid || GetCooldown(gridUid) > TimeSpan.Zero)
            return;

        if (!GetPlanets(ent.Comp).Contains(args.Planet))
            return;

        if (!_prototype.HasIndex<PlanetDestinationPrototype>(args.Planet))
            return;

        ent.Comp.Printing = args.Planet;
        ent.Comp.PrintEnd = _timing.CurTime + ent.Comp.PrintTime;
        _audio.PlayPvs(ent.Comp.PrintSound, ent);

        // The console rattles for the whole thing, humming like a generator and calling out what
        // it is doing as it goes.
        _jitter.AddJitter(ent.Owner, PrintShakeAmplitude, PrintShakeFrequency);
        ent.Comp.PrintStream = _audio.PlayPvs(
            ent.Comp.PrintLoopSound,
            ent,
            AudioParams.Default.WithLoop(true).WithVolume(-6f))?.Entity;

        var step = ent.Comp.PrintTime / (PrintSteps.Length + 1);
        for (var i = 0; i < PrintSteps.Length; i++)
        {
            var message = PrintSteps[i];
            Timer.Spawn(step * (i + 1), () => PopupStep(ent.Owner, message));
        }

        Timer.Spawn(ent.Comp.PrintTime, () => FinishPrint(ent.Owner));
        UpdateUi(ent);
    }

    /// <summary>
    /// Ends the shake and sends the grid on its way, once the full course has been plotted.
    /// </summary>
    private void FinishPrint(EntityUid uid)
    {
        if (TerminatingOrDeleted(uid) || !TryComp<PlanetConsoleComponent>(uid, out var comp))
            return;

        var planetId = comp.Printing;
        comp.Printing = null;

        RemCompDeferred<JitteringComponent>(uid);

        if (comp.PrintStream is { } stream)
        {
            _audio.Stop(stream);
            comp.PrintStream = null;
        }

        if (planetId != null
            && _prototype.TryIndex<PlanetDestinationPrototype>(planetId, out var planet)
            && Transform(uid).GridUid is { } gridUid
            && TryComp<ShuttleComponent>(gridUid, out var shuttle))
        {
            var name = Loc.GetString(planet.Name);
            var mapUid = GetOrCreatePlanet(planet, name, out var arrival);

            if (_shuttle.FTLToCoordinates(gridUid, shuttle, new EntityCoordinates(mapUid, arrival), Angle.Zero))
            {
                // The recharge component is server-only, so it is never dirtied for the network -
                // doing that trips a debug assert. Its data field is saved with the grid instead.
                var cooldown = EnsureComp<PlanetJumpCooldownComponent>(gridUid);
                cooldown.NextJump = _timing.CurTime + comp.Cooldown;
            }
        }

        UpdateUi((uid, comp));
    }

    /// <summary>
    /// Calls out one step of the course, unless it was already over when the timer fired.
    /// </summary>
    private void PopupStep(EntityUid uid, LocId message)
    {
        if (TerminatingOrDeleted(uid) || !TryComp<PlanetConsoleComponent>(uid, out var comp) || comp.Printing == null)
            return;

        _popup.PopupEntity(Loc.GetString(message), uid);
    }

    /// <summary>
    /// Distance between two shuttles sent to the same planet, so they do not land inside each
    /// other.
    /// </summary>
    private const float ArrivalSpacing = 48f;

    /// <summary>
    /// The planet's map, generating it the first time it is asked for. A second course charted to
    /// the same planet flies to the world that is already out there instead of making a new one.
    /// </summary>
    private EntityUid GetOrCreatePlanet(PlanetDestinationPrototype planet, string name, out Vector2 arrival)
    {
        arrival = Vector2.Zero;

        var maps = EntityQueryEnumerator<PlanetMapComponent>();

        while (maps.MoveNext(out var mapUid, out var existing))
        {
            if (existing.Planet != planet.ID || TerminatingOrDeleted(mapUid))
                continue;

            // Each new shuttle is put a little further along so it does not FTL into the one
            // that got here first.
            arrival = new Vector2(existing.Arrivals * ArrivalSpacing, 0f);
            existing.Arrivals++;
            return mapUid;
        }

        return CreatePlanet(planet, name);
    }

    /// <summary>
    /// Generates the planet's map: it is created uninitialised so everything the planet needs can
    /// be attached first, and initialised last, which also leaves it unpaused and ready to fly to.
    /// </summary>
    private EntityUid CreatePlanet(PlanetDestinationPrototype planet, string name)
    {
        var mapUid = _map.CreateMap(out var mapId, runMapInit: false);

        var destination = EnsureComp<FTLDestinationComponent>(mapUid);
        destination.Enabled = true;
        Dirty(mapUid, destination);

        EnsureComp<FTLBeaconComponent>(mapUid);
        _metaData.SetEntityName(mapUid, name);

        _biome.EnsurePlanet(mapUid, _prototype.Index(planet.Biome), mapLight: planet.MapLight);

        // A planet day is ten minutes. EnsurePlanet hands the map the default half-hour cycle, so
        // the shadow of the sun across the ground is shortened here before anyone can set foot on
        // it.
        if (TryComp<SunShadowCycleComponent>(mapUid, out var sunShadow))
        {
            sunShadow.Duration = TimeSpan.FromMinutes(10);
            Dirty(mapUid, sunShadow);
        }

        // Ore and creatures are seeded the way the salvage expeditions do it: marker layers that
        // the biome system reads as it generates the chunks around whoever is on the planet.
        if (TryComp<BiomeComponent>(mapUid, out var biome))
        {
            foreach (var marker in planet.OreMarkers)
            {
                _biome.AddMarkerLayer(mapUid, biome, marker.Id);
            }

            // Every marker is its own layer, each with its own spawn rules - ore replaces the
            // biome's rock, creatures are put down on the open ground.
            foreach (var marker in planet.MobMarkers)
            {
                _biome.AddMarkerLayer(mapUid, biome, marker.Id);
            }
        }

        // EnsurePlanet leaves a breathable room-temperature atmosphere behind, which is wrong for
        // anything that is not a mild planet, so it is replaced with the one from the prototype.
        var moles = new float[Atmospherics.AdjustedNumberOfGases];
        foreach (var (gas, count) in planet.Atmosphere)
        {
            moles[(int) gas] = count;
        }

        _atmos.SetMapAtmosphere(mapUid, false, new GasMixture(moles, Atmospherics.T0C + planet.Temperature));

        _map.InitializeMap(mapId);

        // The planet remembers what it is, so the next course charted to it finds it instead of
        // generating another one.
        var planetMap = EnsureComp<PlanetMapComponent>(mapUid);
        planetMap.Planet = planet.ID;
        planetMap.Arrivals = 1;

        // The planet gets its storms once it is up and running.
        _planetWeather.StartTracking(mapUid, planet);
        return mapUid;
    }

    /// <summary>
    /// Why the console cannot jump right now, or null when it can. The console has to be mounted on
    /// a shuttle, never on a station.
    /// </summary>
    private LocId? GetBlockReason(EntityUid uid)
    {
        if (Transform(uid).GridUid is not { } gridUid)
            return "planet-console-blocked-no-grid";

        if (_station.GetOwningStation(gridUid) != null)
            return "planet-console-blocked-station";

        if (!HasComp<ShuttleComponent>(gridUid))
            return "planet-console-blocked-not-shuttle";

        return null;
    }

    /// <summary>
    /// Time left before the grid may jump again. Stored on the grid so a rebuilt console keeps it.
    /// </summary>
    private TimeSpan GetCooldown(EntityUid gridUid)
    {
        if (!TryComp<PlanetJumpCooldownComponent>(gridUid, out var cooldown))
            return TimeSpan.Zero;

        var left = cooldown.NextJump - _timing.CurTime;
        return left > TimeSpan.Zero ? left : TimeSpan.Zero;
    }

    private List<string> GetPlanets(PlanetConsoleComponent component)
    {
        if (component.Planets.Count > 0)
            return component.Planets.Select(planet => planet.Id).ToList();

        return _prototype.EnumeratePrototypes<PlanetDestinationPrototype>()
            .OrderBy(planet => planet.Order)
            .Select(planet => planet.ID)
            .ToList();
    }

    private void UpdateUi(Entity<PlanetConsoleComponent> ent)
    {
        var remaining = 0f;
        if (ent.Comp.Printing != null)
            remaining = MathF.Max(0f, (float) (ent.Comp.PrintEnd - _timing.CurTime).TotalSeconds);

        var cooldown = 0f;
        var blockReason = string.Empty;

        if (ent.Comp.Printing == null)
        {
            blockReason = GetBlockReason(ent.Owner) ?? string.Empty;

            if (blockReason.Length == 0 && Transform(ent.Owner).GridUid is { } gridUid)
                cooldown = (float) GetCooldown(gridUid).TotalSeconds;
        }

        var state = new PlanetConsoleState(
            GetPlanets(ent.Comp),
            ent.Comp.Printing ?? string.Empty,
            (float) ent.Comp.PrintTime.TotalSeconds,
            remaining,
            cooldown,
            (float) ent.Comp.Cooldown.TotalSeconds,
            blockReason);

        _ui.SetUiState(ent.Owner, PlanetConsoleUiKey.Key, state);
    }
}