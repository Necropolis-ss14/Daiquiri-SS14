using Content.Server.Chat.Managers;
using Content.Shared.Chat;
using Content.Shared.NullLink.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Server._NullLink.PlayerData;

public sealed partial class NullLinkPlayerManager
{
    private sealed record DonationTier(string Title, ulong Role, Color? Color);

    private static readonly Color DonationGreetingColor = Color.FromHex("#FFD54A");
    private const string DonationGreetingText =
        "Огромное спасибо за то что поддерживаете нас, мы будем и дальше старатся развивать сервер. Спасибо что вы с нами.";

    private static readonly CVarDef<string>[] DonationTierCvars =
    [
        NullLinkCCVars.DonationTier1,
        NullLinkCCVars.DonationTier2,
        NullLinkCCVars.DonationTier3,
        NullLinkCCVars.DonationTier4,
        NullLinkCCVars.DonationTier5,
        NullLinkCCVars.DonationTier6,
        NullLinkCCVars.DonationTier7,
        NullLinkCCVars.DonationTier8,
        NullLinkCCVars.DonationTier9,
        NullLinkCCVars.DonationTier10,
    ];

    private DonationTier[] _donationTiers = [];

    private void UpdateDonationTiers(string _)
    {
        var tiers = new List<DonationTier>();

        foreach (var cvar in DonationTierCvars)
        {
            if (TryParseDonationTier(_cfg.GetCVar(cvar), out var tier))
                tiers.Add(tier);
        }

        _donationTiers = [.. tiers];

        foreach (var player in _playerById)
            RebuildTitle(_playerManager.GetSessionById(new NetUserId(player.Key)), player.Value);
    }

    private static bool TryParseDonationTier(string value, out DonationTier tier)
    {
        tier = null!;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var parts = value.Split('|');
        if (parts.Length != 3)
            return false;

        var title = parts[0].Trim();
        if (title.Length == 0 || !ulong.TryParse(parts[1].Trim(), out var role))
            return false;

        var colorText = parts[2].Trim();
        Color? color = null;
        if (colorText.Length > 0)
        {
            if (!Color.TryFromHex(colorText, out var parsed))
                return false;
            color = parsed;
        }

        tier = new DonationTier(title, role, color);
        return true;
    }

    private void RebuildDonation(PlayerData playerData)
    {
        playerData.DonationTitle = null;
        playerData.DonationColor = null;

        foreach (var tier in _donationTiers)
        {
            if (!playerData.Roles.Contains(tier.Role))
                continue;

            playerData.DonationTitle = tier.Title;
            playerData.DonationColor = tier.Color;
            break;
        }

        if (playerData.DonationTitle is not null || playerData.DonationColor is not null)
            GreetDonator(playerData);
    }

    private void GreetDonator(PlayerData playerData)
    {
        if (playerData.DonationGreeted)
            return;

        var session = playerData.Session;
        if (session.Status == SessionStatus.Disconnected)
            return;

        playerData.DonationGreeted = true;

        var wrapped = Loc.GetString("chat-manager-server-wrap-message",
            ("message", FormattedMessage.EscapeText(DonationGreetingText)));
        _chat.ChatMessageToOne(ChatChannel.Server, DonationGreetingText, wrapped, default, false,
            session.Channel, colorOverride: DonationGreetingColor, recordReplay: false);
    }
}
