using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Server._Daiquiri.Lobby;

/// <summary>
/// Daiquiri
/// Хранилище выданных пластинок, переживает перезапуск сервера.
/// </summary>
public sealed partial class VinylGrantManager : IVinylGrants
{
    [Dependency] private IResourceManager _resources = default!;

    private const string GrantsFileName = "vinyl_grants.json";

    private Dictionary<Guid, HashSet<string>>? _grants;

    public bool HasGrant(Guid userId, string vinylId)
    {
        return Grants.TryGetValue(userId, out var vinyls) && vinyls.Contains(vinylId);
    }

    public HashSet<string> GetGrants(Guid userId)
    {
        return Grants.TryGetValue(userId, out var vinyls) ? new HashSet<string>(vinyls) : [];
    }

    public void Grant(Guid userId, string vinylId)
    {
        if (!Grants.TryGetValue(userId, out var vinyls))
        {
            vinyls = new HashSet<string>();
            Grants[userId] = vinyls;
        }
        vinyls.Add(vinylId);
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
            var dir = Path.GetDirectoryName(path);
            if (dir != null)
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, JsonSerializer.Serialize(_grants));
        }
        catch
        {
            // Best effort persistence, never break gameplay.
        }
    }
}
