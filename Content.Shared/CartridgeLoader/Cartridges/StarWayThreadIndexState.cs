using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

/// <summary>
/// One StarWay thread (board root or subthread) as indexed for Googolplex search.
/// </summary>
[Serializable, NetSerializable]
public sealed class StarWayThreadEntry
{
    public string BoardCode;
    public int ThreadId;
    public string Title;
    public string Description;
    public int PostCount;
    public bool IsSubthread;
    /// <summary>Lowercased haystack of title, description and all post texts for keyword search.</summary>
    public string SearchText;

    public StarWayThreadEntry(string boardCode, int threadId, string title, string description, int postCount, bool isSubthread, string searchText)
    {
        BoardCode = boardCode;
        ThreadId = threadId;
        Title = title;
        Description = description;
        PostCount = postCount;
        IsSubthread = isSubthread;
        SearchText = searchText;
    }
}

[Serializable, NetSerializable]
public sealed class StarWayThreadIndexState : BoundUserInterfaceState
{
    public List<StarWayThreadEntry> Threads;

    public StarWayThreadIndexState(List<StarWayThreadEntry> threads)
    {
        Threads = threads;
    }
}
