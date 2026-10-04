using Content.Shared._NullLink;
using Robust.Shared.Player;

namespace Content.Shared._Starlight.Abstract.Conditions;

public sealed partial class MentorRequirement : BaseRequirement
{
    [Dependency] public ISharedNullLinkPlayerRolesReqManager _roles = default!;

    public override string GetRequirementDescription()
    {
        base.GetRequirementDescription();

        return Loc.GetString("requirements-mentor");
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        return _roles.IsMentor(user);
    }
}
