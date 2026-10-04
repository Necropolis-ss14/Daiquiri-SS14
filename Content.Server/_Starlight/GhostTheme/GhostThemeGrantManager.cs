using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Content.Shared._Starlight.GhostTheme;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Server._Starlight.GhostTheme;

/// <summary>
/// Server-side ghost theme grants, persisted to the user data dir so they survive restarts.
/// </summary>
public sealed partial class GhostThemeGrantManager : IGhostThemeGrantManager
{
    [Dependency] private IResourceManager _resources = default!;

    private const string GrantsFileName = "ghost_theme_grants.json";

    private Dictionary<Guid, HashSet<string>>? _grants;

    public bool HasGrant(Guid userId, string themeId)
    {
        return Grants.TryGetValue(userId, out var themes) && themes.Contains(themeId);
    }

    public void Grant(Guid userId, string themeId)
    {
        if (!Grants.TryGetValue(userId, out var themes))
        {
            themes = new HashSet<string>();
            Grants[userId] = themes;
        }
        themes.Add(themeId);
        SaveGrants();
    }

    private Dictionary<Guid, HashSet<string>> Grants
    {
        get
        {
            if (_grants == null)
                _grants = LoadGrants();
            return _grants;
        }
    }

    private string? GrantsPath()
    {
        var root = _resources.UserData.RootDir;
        return root == null ? null : Path.Combine(root, GrantsFileName);
    }

    private Dictionary<Guid, HashSet<string>> LoadGrants()
    {
        try
        {
            var path = GrantsPath();
            if (path != null && File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var data = JsonSerializer.Deserialize<Dictionary<Guid, HashSet<string>>>(json);
                if (data != null)
                    return data;
            }
        }
        catch
        {
            // Corrupt file? Start fresh rather than crash.
        }
        return new Dictionary<Guid, HashSet<string>>();
    }

    private void SaveGrants()
    {
        try
        {
            var path = GrantsPath();
            if (path == null || _grants == null)
                return;
            File.WriteAllText(path, JsonSerializer.Serialize(_grants));
        }
        catch
        {
            // Best effort persistence, never break gameplay.
        }
    }
}
