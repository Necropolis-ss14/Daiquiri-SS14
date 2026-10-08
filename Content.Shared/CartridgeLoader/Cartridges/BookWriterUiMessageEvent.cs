using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class BookWriterUiMessageEvent : CartridgeMessageEvent
{
    public readonly BookWriterUiAction Action;
    public readonly string BookId;
    public readonly string Title;
    public readonly string Description;
    public readonly string CoverColor;
    public readonly string CoverDecor;
    public readonly string CoverIcon;
    public readonly string Genre;
    public readonly bool Draft;
    public readonly List<string> Pages;

    public BookWriterUiMessageEvent(
        BookWriterUiAction action,
        string bookId = "",
        string title = "",
        string description = "",
        string coverColor = "",
        string coverDecor = "",
        string coverIcon = "",
        string genre = "other",
        bool draft = false,
        List<string>? pages = null)
    {
        Action = action;
        BookId = bookId;
        Title = title;
        Description = description;
        CoverColor = coverColor;
        CoverDecor = coverDecor;
        CoverIcon = coverIcon;
        Genre = genre;
        Draft = draft;
        Pages = pages ?? new List<string>();
    }
}

[Serializable, NetSerializable]
public enum BookWriterUiAction
{
    Hello,
    Save,
    Delete,
    Print
}
