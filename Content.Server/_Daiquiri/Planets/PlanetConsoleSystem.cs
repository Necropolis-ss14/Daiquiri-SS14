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
using Content.Shared.Cargo.Components;
using Content.Shared.Light.Components;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Events;
using Content.Shared.Station.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Planets;

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

    private static readonly LocId[] PrintSteps =
    {
        "planet-console-step-routing",
        "planet-console-step-calibration",
        "planet-console-step-safe-path",
        "planet-console-step-coordinates",
        "planet-console-step-beacon",
        "planet-console-step-recording",
    };

    private const float PrintShakeAmplitude = 80f;
    private const float PrintShakeFrequency = 8f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlanetConsoleComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<PlanetConsoleComponent, PlanetConsolePrintMessage>(OnPrintMessage);

        SubscribeLocalEvent<ShuttleConsoleComponent, ShuttleConsoleSurfaceReturnMessage>(OnSurfaceReturn);
    }

    private void OnSurfaceReturn(Entity<ShuttleConsoleComponent> ent, ref ShuttleConsoleSurfaceReturnMessage args)
    {
        if (!TryComp<PilotComponent>(args.Actor, out var pilot) || pilot.Console != ent.Owner)
            return;

        if (Transform(ent.Owner).GridUid is not { } shuttleUid
            || Transform(ent.Owner).MapUid is not { } mapUid
            || !HasComp<PlanetMapComponent>(mapUid)
            || !TryComp<ShuttleComponent>(shuttleUid, out var shuttle)
            || HasComp<FTLComponent>(shuttleUid)
            || !_shuttle.CanFTL(shuttleUid, out _))
        {
            return;
        }

        if (_station.GetOwningStation(shuttleUid) is not { } stationUid
            || !TryComp<StationDataComponent>(stationUid, out var stationData))
        {
            Log.Warning($"No station owns {ToPrettyString(shuttleUid)}, so it cannot return from {ToPrettyString(mapUid)}.");
            return;
        }

        EntityUid? targetGrid = null;
        foreach (var member in stationData.Grids)
        {
            if (!HasComp<TradeStationComponent>(member))
                continue;

            targetGrid = member;
            break;
        }

        targetGrid ??= _station.GetLargestGrid((stationUid, stationData));

        if (targetGrid is not { } target)
        {
            Log.Warning($"No station grid found for {ToPrettyString(shuttleUid)} returning to {ToPrettyString(stationUid)}.");
            return;
        }

        _shuttle.FTLToDock(shuttleUid, shuttle, target);
    }

    private void OnUiOpened(Entity<PlanetConsoleComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnPrintMessage(Entity<PlanetConsoleComponent> ent, ref PlanetConsolePrintMessage args)
    {
        if (ent.Comp.Printing != null)
            return;

        if (GetBlockReason(ent.Owner) is { } blocked)
        {
            _popup.PopupEntity(Loc.GetString(blocked), ent.Owner);
            UpdateUi(ent);
            return;
        }

        if (Transform(ent.Owner).GridUid is not { } gridUid || GetCooldown(gridUid) > TimeSpan.Zero)
            return;

        if (!GetPlanets(ent.Comp).Contains(args.Planet))
            return;

        if (!_prototype.HasIndex<PlanetDestinationPrototype>(args.Planet))
            return;

        ent.Comp.Printing = args.Planet;
        ent.Comp.PrintEnd = _timing.CurTime + ent.Comp.PrintTime;
        _audio.PlayPvs(ent.Comp.PrintSound, ent);

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
                var cooldown = EnsureComp<PlanetJumpCooldownComponent>(gridUid);
                cooldown.NextJump = _timing.CurTime + comp.Cooldown;
            }
        }

        UpdateUi((uid, comp));
    }

    private void PopupStep(EntityUid uid, LocId message)
    {
        if (TerminatingOrDeleted(uid) || !TryComp<PlanetConsoleComponent>(uid, out var comp) || comp.Printing == null)
            return;

        _popup.PopupEntity(Loc.GetString(message), uid);
    }

    private const float ArrivalSpacing = 48f;

    private EntityUid GetOrCreatePlanet(PlanetDestinationPrototype planet, string name, out Vector2 arrival)
    {
        arrival = Vector2.Zero;

        var maps = EntityQueryEnumerator<PlanetMapComponent>();

        while (maps.MoveNext(out var mapUid, out var existing))
        {
            if (existing.Planet != planet.ID || TerminatingOrDeleted(mapUid))
                continue;

            arrival = new Vector2(existing.Arrivals * ArrivalSpacing, 0f);
            existing.Arrivals++;
            return mapUid;
        }

        return CreatePlanet(planet, name);
    }

    private EntityUid CreatePlanet(PlanetDestinationPrototype planet, string name)
    {
        var mapUid = _map.CreateMap(out var mapId, runMapInit: false);

        var destination = EnsureComp<FTLDestinationComponent>(mapUid);
        destination.Enabled = true;
        Dirty(mapUid, destination);

        EnsureComp<FTLBeaconComponent>(mapUid);
        _metaData.SetEntityName(mapUid, name);

        _biome.EnsurePlanet(mapUid, _prototype.Index(planet.Biome), mapLight: planet.MapLight);

        if (TryComp<SunShadowCycleComponent>(mapUid, out var sunShadow))
        {
            sunShadow.Duration = TimeSpan.FromMinutes(10);
            Dirty(mapUid, sunShadow);
        }

        if (TryComp<BiomeComponent>(mapUid, out var biome))
        {
            foreach (var marker in planet.OreMarkers)
            {
                _biome.AddMarkerLayer(mapUid, biome, marker.Id);
            }

            foreach (var marker in planet.MobMarkers)
            {
                _biome.AddMarkerLayer(mapUid, biome, marker.Id);
            }
        }

        var moles = new float[Atmospherics.AdjustedNumberOfGases];
        foreach (var (gas, count) in planet.Atmosphere)
        {
            moles[(int) gas] = count;
        }

        _atmos.SetMapAtmosphere(mapUid, false, new GasMixture(moles, Atmospherics.T0C + planet.Temperature));

        _map.InitializeMap(mapId);

        var planetMap = EnsureComp<PlanetMapComponent>(mapUid);
        planetMap.Planet = planet.ID;
        planetMap.Arrivals = 1;

        _planetWeather.StartTracking(mapUid, planet);
        return mapUid;
    }

    private LocId? GetBlockReason(EntityUid uid)
    {
        if (Transform(uid).GridUid is not { } gridUid)
            return "planet-console-blocked-no-grid";

        // The cargo, salvage and salvage specialist shuttles are station members like the station's
        // own floor is, so belonging to a station says nothing about whether a grid can fly. What
        // tells a shuttle apart is the shuttle component.
        if (!HasComp<ShuttleComponent>(gridUid))
        {
            return _station.GetOwningStation(gridUid) != null
                ? "planet-console-blocked-station"
                : "planet-console-blocked-not-shuttle";
        }

        return null;
    }

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