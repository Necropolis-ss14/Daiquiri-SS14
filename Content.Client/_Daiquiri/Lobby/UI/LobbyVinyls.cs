using System;
using Content.Client._Starlight.Achievement;
using Content.Shared._Daiquiri.Lobby;
using Content.Shared._Starlight.Achievement;
using Robust.Shared.Prototypes;

namespace Content.Client._Daiquiri.Lobby.UI;

/// <summary>
/// Daiquiri
/// Хелперы для пластинок в лобби: пути спрайтов, проверка открытия и описание условий.
/// </summary>
public static class LobbyVinyls
{
    /// <summary>
    /// Daiquiri
    /// Количество кадров покачивания у винила.
    /// </summary>
    public const int FrameCount = 6;

    /// <summary>
    /// Daiquiri
    /// Пластинка по умолчанию, открыта всем.
    /// </summary>
    public const string DefaultId = "Classic";

    public static string Frame(LobbyVinylPrototype vinyl, int index)
    {
        var frame = ((index % FrameCount) + FrameCount) % FrameCount;
        return $"{vinyl.Sprite}_{frame}.png";
    }

    public static string Icon(LobbyVinylPrototype vinyl)
    {
        return $"{vinyl.Sprite}_icon.png";
    }

    public static string[] Frames(LobbyVinylPrototype vinyl)
    {
        var frames = new string[FrameCount];
        for (var i = 0; i < FrameCount; i++)
            frames[i] = Frame(vinyl, i);

        return frames;
    }

    public static bool IsUnlocked(LobbyVinylPrototype vinyl, IClientAchievementManager achievements, TimeSpan playtime, IClientVinylGrants grants)
    {
        if (grants.IsGranted(vinyl.ID))
            return true;

        if (vinyl.GrantOnly)
            return false;

        if (playtime < vinyl.RequiredPlaytime)
            return false;

        if (vinyl.RequiredAchievement is { } achievement && !achievements.IsUnlocked(achievement.Id))
            return false;

        return true;
    }

    public static string? UnlockDescription(LobbyVinylPrototype vinyl, IPrototypeManager prototypes)
    {
        if (vinyl.GrantOnly)
            return Loc.GetString("lobby-vinyl-requires-grant");

        if (vinyl.RequiredAchievement is { } achievementId)
        {
            var achievement = prototypes.TryIndex(achievementId, out AchievementPrototype? achievementProto)
                ? Loc.GetString(achievementProto.Name)
                : achievementId.Id;

            return Loc.GetString("lobby-vinyl-requires-achievement", ("achievement", achievement));
        }

        if (vinyl.RequiredPlaytime > TimeSpan.Zero)
            return Loc.GetString("lobby-vinyl-requires-playtime", ("hours", (int) vinyl.RequiredPlaytime.TotalHours));

        return null;
    }
}
