using Content.Shared._Starlight.Admin;
using Content.Shared.Administration;
using Robust.Shared.Player;

namespace Content.Client._Starlight.Admin;

/// <summary>
/// Client stub: admin flags are evaluated server-side only.
/// </summary>
public sealed class ClientAdminFlagManager : IServerAdminManager
{
    public bool HasFlag(ICommonSession user, AdminFlags flag)
    {
        return false;
    }
}
