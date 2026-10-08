using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Server.CartridgeLoader.Cartridges;

/// <summary>
/// Daiquiri: player-written PDA books, persisted to the server user data dir across rounds.
/// </summary>
public sealed partial class BookLibraryManager
{
    [Dependency] private IResourceManager _resources = default!;

    private const string BooksFileName = "pda_books.json";
    private const int MaxTitleLength = 64;
    private const int MaxDescriptionLength = 140;
    private const int MaxPages = 12;
    private const int MaxPageLength = 12000;
    private const int MaxBooksPerAuthor = 10;
    private const int MaxBooksTotal = 100;

    public static readonly HashSet<string> AllowedCoverColors = new()
    {
        "#1134A6", "#A61111", "#117A11", "#6A117A", "#222222", "#d4af37",
    };

    public static readonly HashSet<string> AllowedCoverDecors = new()
    {
        "decor_spine", "decor_bottom", "decor_diagonal",
        "decor_middle", "decor_vertical_middle",
        "decor_wingette", "decor_wingette_flat", "decor_wingette_circle",
    };

    public static readonly HashSet<string> AllowedGenres = new()
    {
        "other", "scifi", "horror", "detective", "romance", "guide", "history", "poetry",
    };

    public static readonly HashSet<string> AllowedCoverIcons = new()
    {
        "book_icon", "icon_eye", "icon_skull", "icon_stars",
        "icon_temple", "icon_diamond", "icon_magic", "icon_planet",
        "icon_tree", "icon_law", "icon_possum", "icon_time", "icon_text",
    };

    private List<BookRecord>? _books;

    public IReadOnlyList<BookRecord> GetBooks() => Books.AsReadOnly();

    public bool TrySaveBook(
        string authorKey,
        string authorName,
        string title,
        string description,
        string coverColor,
        string coverDecor,
        string coverIcon,
        string genre,
        bool draft,
        string? bookId,
        List<string> pages,
        bool isAdmin,
        out string? error)
    {
        title = title.Trim();
        description = description.Trim();
        error = null;

        var cleanPages = pages.Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

        if (title.Length < 3)
        {
            error = "title-too-short";
            return false;
        }
        if (title.Length > MaxTitleLength
            || description.Length > MaxDescriptionLength
            || cleanPages.Count == 0
            || cleanPages.Count > MaxPages
            || cleanPages.Any(p => p.Length > MaxPageLength)
            || !AllowedCoverColors.Contains(coverColor)
            || !AllowedCoverDecors.Contains(coverDecor)
            || !AllowedCoverIcons.Contains(coverIcon)
            || !AllowedGenres.Contains(genre))
        {
            error = "invalid";
            return false;
        }

        var books = Books;
        var existing = bookId != null ? books.FirstOrDefault(b => b.Id == bookId) : null;
        // Daiquiri: edit/delete only by the creator or an admin.
        if (existing != null && existing.AuthorKey != authorKey && !isAdmin)
        {
            error = "not-yours";
            return false;
        }
        if (existing == null && books.Count(b => b.AuthorKey == authorKey) >= MaxBooksPerAuthor)
        {
            error = "too-many";
            return false;
        }
        if (existing == null && books.Count >= MaxBooksTotal)
        {
            error = "library-full";
            return false;
        }

        if (existing != null)
        {
            existing.Title = title;
            existing.Description = description;
            existing.CoverColor = coverColor;
            existing.CoverDecor = coverDecor;
            existing.CoverIcon = coverIcon;
            existing.Genre = genre;
            existing.Draft = draft;
            existing.Pages = cleanPages;
        }
        else
        {
            books.Add(new BookRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = title,
                AuthorKey = authorKey,
                AuthorName = authorName.Trim(),
                Description = description,
                CoverColor = coverColor,
                CoverDecor = coverDecor,
                CoverIcon = coverIcon,
                Genre = genre,
                Draft = draft,
                Pages = cleanPages,
                Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            });
        }
        SaveBooks();
        return true;
    }

    public bool TryDeleteBook(string authorKey, string id, bool isAdmin)
    {
        var books = Books;
        var book = books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            return false;
        if (book.AuthorKey != authorKey && !isAdmin)
            return false;
        books.Remove(book);
        SaveBooks();
        return true;
    }

    private List<BookRecord> Books
    {
        get
        {
            if (_books == null)
                _books = LoadBooks();
            return _books;
        }
    }

    private string? BooksPath()
    {
        var root = _resources.UserData.RootDir;
        return root == null ? null : Path.Combine(root, BooksFileName);
    }

    private List<BookRecord> LoadBooks()
    {
        try
        {
            var path = BooksPath();
            if (path != null && File.Exists(path))
            {
                var data = JsonSerializer.Deserialize<List<BookRecord>>(File.ReadAllText(path));
                if (data != null)
                    return data;
            }
        }
        catch
        {
            // Corrupt file? Start fresh rather than crash.
        }
        return new List<BookRecord>();
    }

    private void SaveBooks()
    {
        try
        {
            var path = BooksPath();
            if (path != null)
                File.WriteAllText(path, JsonSerializer.Serialize(Books));
        }
        catch
        {
            // Best effort only.
        }
    }

    public sealed class BookRecord
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string AuthorKey { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public string Description { get; set; } = "";
        public string CoverColor { get; set; } = "#1134A6";
        public string CoverDecor { get; set; } = "decor_spine";
        public string CoverIcon { get; set; } = "book_icon";
        public string Genre { get; set; } = "other";
        public bool Draft { get; set; }
        public List<string> Pages { get; set; } = new();
        public long Created { get; set; }
    }
}
