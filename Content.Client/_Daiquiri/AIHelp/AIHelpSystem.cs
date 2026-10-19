#nullable enable
using Content.Shared._Daiquiri.AIHelp;
using JetBrains.Annotations;
using Robust.Shared.Network;

namespace Content.Client._Daiquiri.AIHelp;

[UsedImplicitly]
public sealed partial class AIHelpSystem : SharedAIHelpSystem
{
    /// <summary>
    ///     Raised when the server sends an AI reply or a typing update.
    /// </summary>
    public event Action<AIHelpResponseMessage>? OnAIHelpResponse;

    public event Action<AIHelpAdminLogMessage>? OnAdminLogUpdated;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<AIHelpResponseMessage>(OnResponse);
        SubscribeNetworkEvent<AIHelpAdminLogMessage>(OnAdminLog);
    }

    private void OnResponse(AIHelpResponseMessage message, EntitySessionEventArgs eventArgs)
        => OnAIHelpResponse?.Invoke(message);

    private void OnAdminLog(AIHelpAdminLogMessage message, EntitySessionEventArgs eventArgs)
        => OnAdminLogUpdated?.Invoke(message);

    /// <summary>
    ///     Sends a player message to the server to be answered by the AI.
    /// </summary>
    public void Send(string text)
        => RaiseNetworkEvent(new AIHelpRequestMessage { Text = text });

    /// <summary>
    ///     Asks the server to forget the current conversation history.
    /// </summary>
    public void Reset()
        => RaiseNetworkEvent(new AIHelpResetMessage());

    public void RequestAdminLog()
        => RaiseNetworkEvent(new AIHelpAdminLogRequestMessage());

    public void CancelRequest(int entryId)
        => RaiseNetworkEvent(new AIHelpAdminCancelRequestMessage { EntryId = entryId });

    public void ResetPlayerContext(NetUserId userId)
        => RaiseNetworkEvent(new AIHelpAdminResetRequestMessage { UserId = userId });

    public void SetMuted(NetUserId userId, bool muted)
        => RaiseNetworkEvent(new AIHelpAdminMuteRequestMessage { UserId = userId, Muted = muted });
}
