using Content.Shared._NullLink;
using Content.Shared._Starlight.Admin;
using Content.Shared.Administration;
using Robust.Shared.Player;

namespace Content.Shared._Starlight.Abstract.Conditions;

public sealed partial class MentorRequirement : BaseRequirement
{
    [Dependency] public ISharedNullLinkPlayerRolesReqManager _roles = default!;
    [Dependency] public IServerAdminManager _admins = default!;

    public override string GetRequirementDescription()
    {
        base.GetRequirementDescription();

        return Loc.GetString("requirements-mentor");
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        // Daiquiri: anyone who may answer mentor helps - NullLink mentors or admins with the adminhelp flag.
        return _roles.IsMentor(user) || _admins.HasFlag(user, AdminFlags.Adminhelp);
    }
}
