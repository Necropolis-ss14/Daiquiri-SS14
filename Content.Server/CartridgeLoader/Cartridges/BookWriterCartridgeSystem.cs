using System.Linq;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Robust.Server;
using Content.Shared.Administration;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Content.Shared.Database;
using Content.Shared.Paper;
using Robust.Server.Player;

namespace Content.Server.CartridgeLoader.Cartridges;

/// <summary>
/// Daiquiri: PDA book writer. Books persist on the server across rounds.
/// </summary>
public sealed partial class BookWriterCartridgeSystem : EntitySystem
{
    [Dependency] private CartridgeLoaderSystem _cartridgeLoaderSystem = default!;
    [Dependency] private IAdminLogManager _adminLogger = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IAdminManager _admins = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private BookLibraryManager _library = default!;

    private readonly Dictionary<EntityUid, string> _lastViewer = new();
    private readonly Dictionary<string, long> _lastPrint = new();
    private const long PrintCooldownSeconds = 60;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BookWriterCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
        SubscribeLocalEvent<BookWriterCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
    }

    private void OnUiReady(EntityUid uid, BookWriterCartridgeComponent component, CartridgeUiReadyEvent args)
    {
        _lastViewer.TryGetValue(uid, out var viewer);
        UpdateUiState(uid, args.Loader, viewer);
    }

    private void OnUiMessage(EntityUid uid, BookWriterCartridgeComponent component, CartridgeMessageEvent args)
    {
        if (args is not BookWriterUiMessageEvent message)
            return;

        if (!_players.TryGetSessionByEntity(args.Actor, out var session))
            return;

        // Daiquiri: drafts bind to the player account name, not the character.
        var viewerKey = session.Name;
        _lastViewer[uid] = viewerKey;

        if (message.Action == BookWriterUiAction.Hello)
        {
            UpdateUiState(uid, GetEntity(args.LoaderUid), viewerKey, _admins.HasAdminFlag(session, AdminFlags.Admin));
            return;
        }

        if (message.Action == BookWriterUiAction.Print)
        {
            TryPrintBook(args.Actor, viewerKey, message.BookId);
            UpdateUiState(uid, GetEntity(args.LoaderUid), viewerKey, _admins.HasAdminFlag(session, AdminFlags.Admin));
            return;
        }

        if (message.Action == BookWriterUiAction.Save)
        {
            var isAdmin = _admins.HasAdminFlag(session, AdminFlags.Admin);
            // Daiquiri: ownership key is the account name, display name is the character.
            if (_library.TrySaveBook(
                    viewerKey,
                    MetaData(args.Actor).EntityName,
                    message.Title,
                    message.Description,
                    message.CoverColor,
                    message.CoverDecor,
                    message.CoverIcon,
                    message.Genre,
                    message.Draft,
                    string.IsNullOrEmpty(message.BookId) ? null : message.BookId,
                    message.Pages,
                    isAdmin,
                    out _))
            {
                _adminLogger.Add(LogType.PdaInteract, LogImpact.Low,
                    $"{ToPrettyString(args.Actor)} saved a PDA book '{message.Title}'");
            }
        }
        else
        {
            var isAdmin = _admins.HasAdminFlag(session, AdminFlags.Admin);
            if (_library.TryDeleteBook(viewerKey, message.BookId, isAdmin))
            {
                _adminLogger.Add(LogType.PdaInteract, LogImpact.Low,
                    $"{ToPrettyString(args.Actor)} deleted a PDA book '{message.BookId}'");
            }
        }

        UpdateUiState(uid, GetEntity(args.LoaderUid), viewerKey, _admins.HasAdminFlag(session, AdminFlags.Admin));
    }

    /// <summary>
    /// Daiquiri: spawn a physical book copy. Once per minute per player.
    /// </summary>
    private void TryPrintBook(EntityUid actor, string viewerKey, string bookId)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (_lastPrint.TryGetValue(viewerKey, out var last) && now - last < PrintCooldownSeconds)
            return;

        var book = _library.GetBooks().FirstOrDefault(b => b.Id == bookId);
        if (book == null || book.Pages.Count == 0)
            return;
        if (book.Draft && book.AuthorKey != viewerKey)
            return;

        // Daiquiri: old records lack decor — default like in the list view.
        var decor = string.IsNullOrEmpty(book.CoverDecor) ? "decor_spine" : book.CoverDecor;
        var variant = BookCoverVariants.Variants.FirstOrDefault(v =>
            v.Color == book.CoverColor && v.Decor == decor && v.Icon == book.CoverIcon);
        if (variant == default)
            return;

        _lastPrint[viewerKey] = now;
        var coords = Transform(actor).MapPosition;
        var printed = EntityManager.SpawnEntity(variant.Proto, coords);

        var meta = MetaData(printed);
        _metaData.SetEntityName(printed, book.Title, meta);
        _metaData.SetEntityDescription(printed, book.Description, meta);

        if (TryComp<PaperComponent>(printed, out var paper))
        {
            // Daiquiri: formal page separation in printed copies.
            var body = book.Pages[0];
            for (var i = 1; i < book.Pages.Count; i++)
            {
                body += $"\n\n{new string('-', 20)}\n" +
                    $"{Loc.GetString("book-writer-printed-page", ("n", i + 1))}\n" +
                    $"{new string('-', 20)}\n\n" + book.Pages[i];
            }
            paper.Content = body;
            Dirty(printed, paper);
        }

        _adminLogger.Add(LogType.PdaInteract, LogImpact.Low,
            $"{ToPrettyString(actor)} printed a PDA book '{book.Title}'");
    }

    private void UpdateUiState(EntityUid uid, EntityUid loaderUid, string? viewerKey, bool viewerAdmin = false)
    {
        long cooldown = 0;
        if (!string.IsNullOrEmpty(viewerKey)
            && _lastPrint.TryGetValue(viewerKey, out var last))
        {
            var left = PrintCooldownSeconds - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - last);
            if (left > 0)
                cooldown = left;
        }
        var books = _library.GetBooks()
            .Where(b => !b.Draft || (!string.IsNullOrEmpty(viewerKey) && b.AuthorKey == viewerKey))
            .Select(b => new BookWriterBookInfo(
                b.Id, b.Title, b.AuthorName, b.Description, b.CoverColor,
                string.IsNullOrEmpty(b.CoverDecor) ? "decor_spine" : b.CoverDecor,
                b.CoverIcon,
                string.IsNullOrEmpty(b.Genre) ? "other" : b.Genre,
                b.Draft,
                viewerAdmin || (!string.IsNullOrEmpty(viewerKey) && b.AuthorKey == viewerKey),
                b.Pages))
            .ToList();
        _cartridgeLoaderSystem?.UpdateCartridgeUiState(loaderUid, new BookWriterUiState(books, cooldown));
    }
}
