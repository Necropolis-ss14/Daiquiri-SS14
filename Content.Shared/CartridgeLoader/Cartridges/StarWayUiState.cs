using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class StarWayUiState : BoundUserInterfaceState
{
    public string BoardCode;
    public int ThreadId;
    public int ParentThreadId;
    public string ThreadTitle;
    public string ThreadDescription;
    public List<StarWayPostInfo> Posts;
    public List<StarWaySubthreadInfo> Subthreads;

    public StarWayUiState(
        string boardCode,
        int threadId,
        int parentThreadId,
        string threadTitle,
        string threadDescription,
        List<StarWayPostInfo> posts,
        List<StarWaySubthreadInfo> subthreads)
    {
        BoardCode = boardCode;
        ThreadId = threadId;
        ParentThreadId = parentThreadId;
        ThreadTitle = threadTitle;
        ThreadDescription = threadDescription;
        Posts = posts;
        Subthreads = subthreads;
    }
}

[Serializable, NetSerializable]
public sealed class StarWaySubthreadInfo
{
    public int Id;
    public string Title;
    public int PostCount;

    public StarWaySubthreadInfo(int id, string title, int postCount)
    {
        Id = id;
        Title = title;
        PostCount = postCount;
    }
}

[Serializable, NetSerializable]
public sealed class StarWayPostInfo
{
    public int Number;
    public string Author;
    public string Text;
    public string Timestamp;
    /// <summary>Account name of the poster, shown to admins only.</summary>
    public string RealAuthor;

    public StarWayPostInfo(int number, string author, string text, string timestamp, string realAuthor = "")
    {
        Number = number;
        Author = author;
        Text = text;
        Timestamp = timestamp;
        RealAuthor = realAuthor;
    }
}
