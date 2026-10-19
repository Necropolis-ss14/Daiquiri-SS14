using System;
using System.Linq;
using Content.Client._Daiquiri.Lobby.UI;
using Content.Client._Starlight.Achievement;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.UserInterface.Systems.Chat;
using Content.Shared._Daiquiri.CCVar;
using Content.Shared._Daiquiri.Lobby;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._Daiquiri.Lobby;

public sealed partial class VinylUnlockNotifier : EntitySystem
{
    [Dependency] private IClientAchievementManager _achievements = default!;
    [Dependency] private JobRequirementsManager _playtime = default!;
    [Dependency] private IClientVinylGrants _vinylGrants = default!;
    [Dependency] private IPrototypeManager _prototypes = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IUserInterfaceManager _ui = default!;
    [Dependency] private SharedAudioSystem _audio = default!;

    private static readonly SoundPathSpecifier _unlockSound = new("/Audio/_Daiquiri/Lobby/vinyl_unlock.ogg");

    public override void Initialize()
    {
        base.Initialize();

        _achievements.AchievementsUpdated += CheckUnlocks;
        _playtime.Updated += CheckUnlocks;
        _vinylGrants.Updated += CheckUnlocks;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _achievements.AchievementsUpdated -= CheckUnlocks;
        _playtime.Updated -= CheckUnlocks;
        _vinylGrants.Updated -= CheckUnlocks;
    }

    private void CheckUnlocks()
    {
        var announced = _cfg.GetCVar(DaiquiriCCVars.LobbyVinylAnnounced)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();

        var playtime = _playtime.FetchOverallPlaytime();
        var changed = false;

        var vinyls = _prototypes.EnumeratePrototypes<LobbyVinylPrototype>()
            .OrderBy(vinyl => vinyl.RequiredPlaytime)
            .ThenBy(vinyl => vinyl.ID, StringComparer.Ordinal);

        foreach (var vinyl in vinyls)
        {
            if (vinyl.ID == LobbyVinyls.DefaultId)
                continue;

            if (announced.Contains(vinyl.ID) || !LobbyVinyls.IsUnlocked(vinyl, _achievements, playtime, _vinylGrants))
                continue;

            announced.Add(vinyl.ID);
            changed = true;
            Announce(vinyl);
        }

        if (changed)
        {
            _cfg.SetCVar(DaiquiriCCVars.LobbyVinylAnnounced, string.Join(',', announced));
            _audio.PlayGlobal(_unlockSound, Filter.Local(), false, AudioParams.Default.WithVolume(-2f));
        }
    }

    private void Announce(LobbyVinylPrototype vinyl)
    {
        var text = Loc.GetString("lobby-vinyl-unlocked-chat", ("vinyl", Loc.GetString(vinyl.Name)));
        var message = new ChatMessage(ChatChannel.Notifications, text, FormattedMessage.EscapeText(text),
            NetEntity.Invalid, null, colorOverride: Color.Gold)
        {
            Read = true,
        };

        _ui.GetUIController<ChatUIController>().ProcessChatMessage(message, false);
    }
}
