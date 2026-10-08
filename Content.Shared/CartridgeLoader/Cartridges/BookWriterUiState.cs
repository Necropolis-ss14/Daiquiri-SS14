using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class BookWriterUiState : BoundUserInterfaceState
{
    public List<BookWriterBookInfo> Books;
    public long PrintCooldown;

    public BookWriterUiState(List<BookWriterBookInfo> books, long printCooldown = 0)
    {
        Books = books;
        PrintCooldown = printCooldown;
    }
}

[Serializable, NetSerializable]
public sealed class BookWriterBookInfo
{
    public string Id;
    public string Title;
    public string Author;
    public string Description;
    public string CoverColor;
    public string CoverDecor;
    public string CoverIcon;
    public string Genre;
    public bool Draft;
    public bool Editable;
    public List<string> Pages;

    public BookWriterBookInfo(
        string id,
        string title,
        string author,
        string description,
        string coverColor,
        string coverDecor,
        string coverIcon,
        string genre,
        bool draft,
        bool editable,
        List<string> pages)
    {
        Id = id;
        Title = title;
        Author = author;
        Description = description;
        CoverColor = coverColor;
        CoverDecor = coverDecor;
        CoverIcon = coverIcon;
        Genre = genre;
        Draft = draft;
        Editable = editable;
        Pages = pages;
    }
}
