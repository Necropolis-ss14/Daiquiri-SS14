#nullable enable
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.AIHelp;

/// <summary>
///     Shared part of the AI help system. Holds the network messages used between the client and the server.
/// </summary>
public abstract class SharedAIHelpSystem : EntitySystem
{
    /// <summary>
    ///     Fake user id used for system-generated messages.
    /// </summary>
    public static NetUserId SystemUserId { get; } = new(Guid.Empty);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<AIHelpRequestMessage>(OnAIHelpRequest);
        SubscribeNetworkEvent<AIHelpResetMessage>(OnAIHelpReset);
    }

    /// <summary>
    ///     Sent by the client when the player submits a message to the AI. Handled on the server only.
    /// </summary>
    protected virtual void OnAIHelpRequest(AIHelpRequestMessage message, EntitySessionEventArgs eventArgs)
    {
    }

    /// <summary>
    ///     Sent by the client to clear the conversation history. Handled on the server only.
    /// </summary>
    protected virtual void OnAIHelpReset(AIHelpResetMessage message, EntitySessionEventArgs eventArgs)
    {
    }

    [Serializable, NetSerializable]
    public sealed class AIHelpRequestMessage() : EntityEventArgs
    {
        public required string Text { get; init; }
    }

    [Serializable, NetSerializable]
    public sealed class AIHelpResetMessage() : EntityEventArgs
    {
    }

    /// <summary>
    ///     Sent by the server with the AI answer, or to indicate that the AI is still typing.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class AIHelpResponseMessage() : EntityEventArgs
    {
        public required string Text { get; init; }

        /// <summary>
        ///     True when the message is a system/error notice rather than an actual AI reply.
        /// </summary>
        public bool IsError { get; init; }

        /// <summary>
        ///     True while the AI is generating an answer, used for the "typing" indicator.
        /// </summary>
        public bool Typing { get; init; }
    }
}
