using System;
using Content.Shared._Starlight.GhostTheme;

namespace Content.Client._Starlight.GhostTheme;

/// <summary>
/// Client stub: grants are evaluated server-side only.
/// </summary>
public sealed class ClientGhostThemeGrantManager : IGhostThemeGrantManager
{
    public bool HasGrant(Guid userId, string themeId)
    {
        return false;
    }

    public void Grant(Guid userId, string themeId)
    {
    }
}
