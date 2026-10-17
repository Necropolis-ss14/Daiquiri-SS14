#nullable enable
using Content.Shared._Starlight.AIHelp;
using JetBrains.Annotations;

namespace Content.Client._Starlight.AIHelp;

[UsedImplicitly]
public sealed partial class AIHelpSystem : SharedAIHelpSystem
{
    /// <summary>
    ///     Raised when the server sends an AI reply or a typing update.
    /// </summary>
    public event Action<AIHelpResponseMessage>? OnAIHelpResponse;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<AIHelpResponseMessage>(OnResponse);
    }

    private void OnResponse(AIHelpResponseMessage message, EntitySessionEventArgs eventArgs)
        => OnAIHelpResponse?.Invoke(message);

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
}
