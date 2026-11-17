using System.Numerics;
using Content.Server.Body.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Placeable;
using Content.Shared.Temperature.Components;
using Robust.Shared.Maths;

namespace Content.Server._Daiquiri.Campfire;

public sealed partial class CampfireSystem : EntitySystem
{
    [Dependency] private TemperatureSystem _temperature = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    private readonly List<(Vector2 Position, CampfireComponent Campfire)> _campfires = new();

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _campfires.Clear();

        var campfireQuery = EntityQueryEnumerator<CampfireComponent, TransformComponent>();
        while (campfireQuery.MoveNext(out _, out var campfire, out var xform))
        {
            _campfires.Add((_transform.GetWorldPosition(xform), campfire));
        }

        if (_campfires.Count == 0)
            return;

        WarmNearbyMobs(frameTime);
        HeatPlacedItems(frameTime);
    }

    private void WarmNearbyMobs(float frameTime)
    {
        var mobQuery = EntityQueryEnumerator<TemperatureComponent, MobStateComponent, TransformComponent>();
        while (mobQuery.MoveNext(out var uid, out var temperature, out _, out var xform))
        {
            var position = _transform.GetWorldPosition(xform);

            foreach (var (campfirePosition, campfire) in _campfires)
            {
                if ((position - campfirePosition).LengthSquared() > campfire.Radius * campfire.Radius)
                    continue;

                var factor = Math.Clamp(campfire.WarmRate * frameTime, 0f, 1f);
                var target = campfire.TargetTemperature;

                if (TryComp<ThermalRegulatorComponent>(uid, out var regulator))
                    target = regulator.NormalBodyTemperature;

                temperature.CurrentTemperature =
                    MathHelper.Lerp(temperature.CurrentTemperature, target, factor);

                break;
            }
        }
    }

    private void HeatPlacedItems(float frameTime)
    {
        var placerQuery = EntityQueryEnumerator<CampfireComponent, ItemPlacerComponent>();
        while (placerQuery.MoveNext(out _, out var campfire, out var placer))
        {
            var energy = campfire.HeatPower * frameTime;

            foreach (var placed in placer.PlacedEntities)
            {
                _temperature.ChangeHeat(placed, energy);
            }
        }
    }
}