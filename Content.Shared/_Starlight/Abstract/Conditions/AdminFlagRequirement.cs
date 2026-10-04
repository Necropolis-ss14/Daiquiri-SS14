using Content.Shared._Starlight.Admin;
using Content.Shared.Administration;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Abstract.Conditions;

public sealed partial class AdminFlagRequirement : BaseRequirement
{
    [Dependency] public IServerAdminManager _admins = default!;

    [DataField(required: true)]
    public AdminFlags Flag;

    public override string GetRequirementDescription()
    {
        base.GetRequirementDescription();

        return Loc.GetString("requirements-admin-flag");
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        return _admins.HasFlag(user, Flag);
    }
}
