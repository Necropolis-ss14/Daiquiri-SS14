using Content.Shared._Starlight.GhostTheme;
using Robust.Shared.Player;

namespace Content.Shared._Starlight.Abstract.Conditions;

public sealed partial class GrantedGhostThemeRequirement : BaseRequirement
{
    [Dependency] public IGhostThemeGrantManager _grants = default!;

    [DataField(required: true)]
    public string Theme = string.Empty;

    public override string GetRequirementDescription()
    {
        base.GetRequirementDescription();

        return Loc.GetString("requirements-granted-ghost-theme");
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        return _grants.HasGrant(user.UserId, Theme);
    }
}
