using System.IO;
using System.Linq;
using System.Text.Json;
using Robust.Shared.ContentPack;
using Starlight.NullLink;

namespace Content.Server._NullLink.PlayerData;

public sealed partial class NullLinkPlayerManager
{
    [Dependency] private IResourceManager _resources = default!;

    private const string LocalAchievementsFileName = "local_achievements.json";

    private Dictionary<Guid, LocalPlayerAchievements>? _localAchievements;

    private Dictionary<Guid, LocalPlayerAchievements> LocalAchievements
    {
        get
        {
            if (_localAchievements == null)
                _localAchievements = LoadLocalAchievements();
            return _localAchievements;
        }
    }

    private string? LocalAchievementsPath()
    {
        var root = _resources.UserData.RootDir;
        return root == null ? null : Path.Combine(root, LocalAchievementsFileName);
    }

    private Dictionary<Guid, LocalPlayerAchievements> LoadLocalAchievements()
    {
        try
        {
            var path = LocalAchievementsPath();
            if (path != null && File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var data = JsonSerializer.Deserialize<Dictionary<Guid, LocalPlayerAchievements>>(json);
                if (data != null)
                    return data;
            }
        }
        catch
        {
            // Corrupt file? Start fresh rather than crash.
        }
        return new Dictionary<Guid, LocalPlayerAchievements>();
    }

    private void SaveLocalAchievements()
    {
        try
        {
            var path = LocalAchievementsPath();
            if (path == null || _localAchievements == null)
                return;
            var dir = Path.GetDirectoryName(path);
            if (dir != null)
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, JsonSerializer.Serialize(_localAchievements));
        }
        catch
        {
            // Best effort persistence, never break gameplay.
        }
    }

    private void ApplyStoredLocalAchievements(Guid userId)
    {
        if (!_playerById.TryGetValue(userId, out var playerData))
            return;

        if (!LocalAchievements.TryGetValue(userId, out var stored))
            return;

        lock (playerData.AchievementSyncRoot)
        {
            var achievements = playerData.UnlockedAchievements.ToHashSet();
            foreach (var (achievementId, record) in stored.Unlocked)
            {
                achievements.RemoveWhere(achievement => achievement.AchievementId == achievementId);
                achievements.Add(new Achievement
                {
                    AchievementId = achievementId,
                    GrantingServer = record.Server ?? string.Empty,
                    UnlockingCharacter = record.CharacterName ?? string.Empty,
                    UnlockTime = record.UnlockTime,
                });
            }
            playerData.UnlockedAchievements = [.. achievements];

            foreach (var (key, value) in stored.Progress)
            {
                if (!playerData.AchievementProgress.TryGetValue(key, out var current) || value > current)
                    playerData.AchievementProgress[key] = value;
            }
        }
    }

    private void PersistLocalAchievements(Guid userId)
    {
        if (!LocalAchievementsEnabled())
            return;

        if (!_playerById.TryGetValue(userId, out var playerData))
            return;

        var data = new LocalPlayerAchievements();
        lock (playerData.AchievementSyncRoot)
        {
            if (!playerData.AchievementCacheHydrated)
                return;

            foreach (var achievement in playerData.UnlockedAchievements)
            {
                data.Unlocked[achievement.AchievementId] = new LocalAchievementRecord
                {
                    CharacterName = achievement.UnlockingCharacter,
                    Server = achievement.GrantingServer,
                    UnlockTime = achievement.UnlockTime,
                };
            }

            foreach (var (key, value) in playerData.AchievementProgress)
                data.Progress[key] = value;
        }

        LocalAchievements[userId] = data;
        SaveLocalAchievements();
    }
}

public sealed class LocalPlayerAchievements
{
    public Dictionary<string, LocalAchievementRecord> Unlocked { get; set; } = new();
    public Dictionary<string, double> Progress { get; set; } = new();
}

public sealed class LocalAchievementRecord
{
    public string? CharacterName { get; set; }
    public string? Server { get; set; }
    public DateTime UnlockTime { get; set; }
}
