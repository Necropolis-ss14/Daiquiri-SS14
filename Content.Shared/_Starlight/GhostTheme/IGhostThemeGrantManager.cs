using System;
using System.Collections.Generic;
using Robust.Shared.Network;

namespace Content.Shared._Starlight.GhostTheme;

/// <summary>
/// Server-side grants of ghost themes (see ghosttheme command).
/// </summary>
public interface IGhostThemeGrantManager
{
    bool HasGrant(Guid userId, string themeId);
    void Grant(Guid userId, string themeId);
}
