using Content.Shared.Administration;
using Robust.Shared.Player;

namespace Content.Shared._Starlight.Admin;

/// <summary>
/// Server admin-flag checks, bridged to shared code.
/// </summary>
public interface IServerAdminManager
{
    bool HasFlag(ICommonSession user, AdminFlags flag);
}
