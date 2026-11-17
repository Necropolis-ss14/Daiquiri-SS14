using Content.Server.Atmos.EntitySystems;
using Content.Shared._Lavaland.Atmos;
using Content.Shared.Atmos;

namespace Content.Server._Lavaland.Atmos;

public sealed partial class LavalandAtmosphereSystem : SharedLavalandAtmosphereSystem
{
    [Dependency] private AtmosphereSystem _atmos = default!;

    public override GasMixture? GetTileMixture(Entity<TransformComponent?> ent)
    {
        return _atmos.GetTileMixture(ent);
    }
}
