using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class StarWayUiMessageEvent : CartridgeMessageEvent
{
    public readonly StarWayUiAction Action;
    public readonly string BoardCode;
    public readonly int ThreadId;
    public readonly string Author;
    public readonly string Text;
    public readonly string Title;
    public readonly int PostNumber;

    public StarWayUiMessageEvent(
        StarWayUiAction action,
        string boardCode = "",
        int threadId = 0,
        string author = "",
        string text = "",
        string title = "",
        int postNumber = 0)
    {
        Action = action;
        BoardCode = boardCode;
        ThreadId = threadId;
        Author = author;
        Text = text;
        Title = title;
        PostNumber = postNumber;
    }
}

[Serializable, NetSerializable]
public enum StarWayUiAction
{
    OpenThread,
    PostMessage,
    Refresh,
    CreateSubthread,
    DeletePost,
    ListThreads
}
