using System;
using Content.Shared._Starlight.Achievement;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Lobby;

/// <summary>
/// Daiquiri
/// A record (пластинка) that the player can pick for the lobby music player.
/// The choice is purely cosmetic and stays on the client.
/// </summary>
[Prototype]
public sealed partial class LobbyVinylPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Display name shown in the vinyl picker.
    /// </summary>
    [DataField(required: true)]
    public LocId Name { get; private set; } = default!;

    /// <summary>
    /// Daiquiri
    /// Path of the sprite without the frame suffix: the rocking animation lives in
    /// "{Sprite}_0.png" .. "{Sprite}_5.png" and the button icon in "{Sprite}_icon.png".
    /// </summary>
    [DataField(required: true)]
    public string Sprite { get; private set; } = string.Empty;

    /// <summary>
    /// Daiquiri
    /// Overall playtime on the server needed to open the record.
    /// </summary>
    [DataField]
    public TimeSpan RequiredPlaytime { get; private set; }

    /// <summary>
    /// Daiquiri
    /// Achievement needed to open the record.
    /// </summary>
    [DataField]
    public ProtoId<AchievementPrototype>? RequiredAchievement { get; private set; }
}
