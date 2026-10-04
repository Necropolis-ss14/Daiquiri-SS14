using Content.Server.Administration.Managers;
using Content.Shared._Starlight.Admin;
using Content.Shared.Administration;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Server._Starlight.Admin;

public sealed partial class ServerAdminManager : IServerAdminManager
{
    [Dependency] private IAdminManager _admins = default!;

    public bool HasFlag(ICommonSession user, AdminFlags flag)
    {
        return _admins.GetAdminData(user)?.HasFlag(flag) ?? false;
    }
}
