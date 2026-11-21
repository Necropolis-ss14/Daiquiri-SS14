// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityEffects;

public sealed partial class ScaleFixtures : EntityEffectBase<ScaleFixtures>
{
    [DataField(required: true)]
    public float Multiplier;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
        => null;
}

public sealed partial class ScaleFixturesEffectSystem : EntityEffectSystem<FixturesComponent, ScaleFixtures>
{
    [Dependency] private SharedPhysicsSystem _physics = default!;

    protected override void Effect(Entity<FixturesComponent> ent, ref EntityEffectEvent<ScaleFixtures> args)
    {
        _physics.ScaleFixtures(ent.AsNullable(), args.Effect.Multiplier);
    }
}
