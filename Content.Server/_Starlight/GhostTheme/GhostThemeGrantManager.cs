using System;
using System.Collections.Generic;
using Content.Shared._Starlight.GhostTheme;

namespace Content.Server._Starlight.GhostTheme;

public sealed class GhostThemeGrantManager : IGhostThemeGrantManager
{
    private readonly Dictionary<Guid, HashSet<string>> _grants = new();

    public bool HasGrant(Guid userId, string themeId)
    {
        return _grants.TryGetValue(userId, out var themes) && themes.Contains(themeId);
    }

    public void Grant(Guid userId, string themeId)
    {
        if (!_grants.TryGetValue(userId, out var themes))
        {
            themes = new HashSet<string>();
            _grants[userId] = themes;
        }
        themes.Add(themeId);
    }
}
