// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage.Systems;
using Content.Shared._Daiquiri.Harpy.Flight;

namespace Content.Server._Daiquiri.Harpy.Flight;

public sealed partial class FlightSystem : SharedFlightSystem
{
    [Dependency] private SharedStaminaSystem _stamina = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FlightComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (!component.On)
                continue;

            _stamina.TakeStaminaDamage(uid, component.StaminaDrainRate * frameTime, visual: false);
        }
    }
}
