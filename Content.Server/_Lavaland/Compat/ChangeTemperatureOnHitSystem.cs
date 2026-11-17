// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Temperature.Systems;
using Content.Shared._Lavaland.Compat;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server._Lavaland.Compat;

public sealed partial class ChangeTemperatureOnHitSystem : EntitySystem
{
    [Dependency] private TemperatureSystem _temperature = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangeTemperatureOnHitComponent, MeleeHitEvent>(OnHit);
    }

    private void OnHit(Entity<ChangeTemperatureOnHitComponent> ent, ref MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        foreach (var target in args.HitEntities)
        {
            _temperature.ChangeHeat(target, ent.Comp.Heat, ent.Comp.IgnoreResistances);
        }
    }
}
