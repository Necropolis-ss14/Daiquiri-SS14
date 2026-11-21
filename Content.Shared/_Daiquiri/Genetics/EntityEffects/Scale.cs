// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.EntityEffects;
using Content.Shared.Sprite;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Genetics.EntityEffects;

public sealed partial class Scale : EntityEffectBase<Scale>
{
    [DataField(required: true)]
    public Vector2 Multiplier;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
        => loc.GetString("entity-effect-guidebook-scale-entity", ("chance", Probability), ("x", Multiplier.X), ("y", Multiplier.Y));
}

public sealed partial class ScaleEffectSystem : EntityEffectSystem<TransformComponent, Scale>
{
    [Dependency] private SharedScaleVisualsSystem _scale = default!;

    protected override void Effect(Entity<TransformComponent> ent, ref EntityEffectEvent<Scale> args)
    {
        var scale = _scale.GetSpriteScale(ent.Owner);
        var factors = args.Effect.Multiplier;
        _scale.SetSpriteScale(ent.Owner, Vector2.Multiply(scale, factors));
    }
}
