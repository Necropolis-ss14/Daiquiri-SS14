using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class RevertPolymorph : EntityEffectBase<RevertPolymorph>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
        => loc.GetString("entity-effect-guidebook-revert-polymorph", ("chance", Probability));
}
