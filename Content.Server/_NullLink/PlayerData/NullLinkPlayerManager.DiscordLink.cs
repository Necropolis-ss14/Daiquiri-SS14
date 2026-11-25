using Content.Server._NullLink.Helpers;
using Content.Server.EUI;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server._NullLink.PlayerData;

public sealed partial class NullLinkPlayerManager
{
    [Dependency] private EuiManager _euiManager = default!;

    private readonly Dictionary<ICommonSession, DiscordLinkEui> _discordPromptOpen = [];

    // ask the cluster for the live link state and nudge the player if no discord is tied to the account
    private void CheckDiscordLink(ICommonSession session)
    {
        if (_discordPromptOpen.ContainsKey(session))
            return;

        var url = GetActiveDiscordAuthUrl(session.UserId.ToString());
        if (string.IsNullOrEmpty(url))
            return;

        if (IsBridgeEnabled)
        {
            if (!IsDiscordLinked(session.UserId))
                OpenDiscordPrompt(session, url);
            return;
        }

        if (!_actors.TryGetServerGrain(out var serverGrain))
            return;

        serverGrain.GetPlayerDiscordId(session.UserId)
            .Then(discordId =>
            {
                if (discordId == 0)
                    _taskManager.RunOnMainThread(() => OpenDiscordPrompt(session, url));
            })
            .FireAndForget(err => _sawmill.Error($"Discord link check failed for {session.UserId}: {err}"));
    }

    private void OpenDiscordPrompt(ICommonSession session, string url)
    {
        if (session.Status == SessionStatus.Disconnected || _discordPromptOpen.ContainsKey(session))
            return;

        var eui = new DiscordLinkEui(this, url);
        _discordPromptOpen[session] = eui;
        _euiManager.OpenEui(eui, session);
        eui.StateDirty();
    }

    internal void OnDiscordPromptClosed(ICommonSession session)
        => _discordPromptOpen.Remove(session);

    internal void CloseDiscordPrompt(ICommonSession session)
    {
        if (_discordPromptOpen.Remove(session, out var eui))
            _euiManager.CloseEui(eui);
    }
}
