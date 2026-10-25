using System.IO;
using System.Text.Json;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.CartridgeLoader;
using Content.Server.Chat.Managers;
using Content.Server.Discord;
using Content.Shared.CCVar;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Content.Shared.Database;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._Daiquiri.CartridgeLoader.Cartridges;

/// <summary>
/// Server-side system for the StarWay forum cartridge.
/// Keeps the per-board thread tree (a board is the root thread, posts can spawn subthreads)
/// and pushes updated state to every client currently looking at the same thread.
/// </summary>
public sealed partial class StarWayCartridgeSystem : EntitySystem
{
    [Dependency] private CartridgeLoaderSystem _cartridgeLoaderSystem = default!;
    [Dependency] private IAdminLogManager _adminLogger = default!;
    [Dependency] private IAdminManager _adminManager = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IPrototypeManager _protoManager = default!;
    [Dependency] private IResourceManager _res = default!;
    [Dependency] private IChatManager _chat = default!;
    [Dependency] private DiscordWebhook _discord = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private string _webhookUrl = string.Empty;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("starway");
    private static readonly ResPath ForumSavePath = new("/starway_forum.json");

    private sealed class ThreadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PostDto> Posts { get; set; } = new();
        public List<ThreadDto> Children { get; set; } = new();
    }

    private sealed class PostDto
    {
        public int Number { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string RealAuthor { get; set; } = string.Empty;
    }

    private sealed class ThreadNode
    {
        public int Id;
        public string Title = string.Empty;
        public string Description = string.Empty;
        public readonly List<StarWayPostInfo> Posts = new();
        public readonly List<ThreadNode> Children = new();
    }

    private static readonly (string Code, string Name, string Description, string[] Posts)[] BoardSeeds =
    {
        ("РНД", "Научный отдел", "Артефакты, токсины и опыты, которые лучше не повторять.", new[] { "Кто-нибудь уже вскрыл новый артефакт?", "Да, и теперь у обезьяны три руки." }),
        ("КМД", "Командование", "Приказы, брифинги и капитанский мостик.", new[] { "Капитан опять объявил красный код без причины.", "Значит, снова обычный вторник." }),
        ("ИНЖ", "Инженерия", "Двигатели, проводка, сингулярность и вечные поломки.", new[] { "Сингулярность стабильна. Наверное.", "Кто выключил эмиттеры?!" }),
        ("ОБЩ", "Общий канал", "Болталка станции: новости, слухи и поиск компании на смену.", new[] { "Кто идёт в бар после смены?", "Я, но сначала дотащу отчёт." }),
        ("СЕР", "Сервисный отдел", "Кухня, бар, ботаника и всё, что делает смену уютной.", new[] { "Новый кок обещает борщ.", "Уже боюсь." }),
        ("МЕД", "Медицина", "Клонирование, хирургия, вирусология и очередь в медбей.", new[] { "Кто-нибудь видел второй сканер?", "Он в техтоннеле, не спрашивай." }),
        ("СБ", "Служба безопасности", "Патрули, брифинги, розыск и разборки с клоунами.", new[] { "Клоун украл тазер из арсенала.", "Уже не смешно." }),
        ("ЮР", "Юридический отдел", "Законы станции, суды и жалобы на командование.", new[] { "Подаю иск на ИИ за нарушение приватности.", "Сначала заполни форму 12-Б." }),
        ("СНАБ", "Снабжение", "Заказы, шаттл, кредиты и вечный дефицит.", new[] { "Когда придёт заказ с инструментами?", "Через два часа. Как и вчера." }),
        ("КАРГО", "Карго", "Разгрузка, контрабанда и торговля с аванпостами.", new[] { "В ящике снова не то, что заказывали.", "Зато прибыль." }),
        ("ШАХТ", "Шахтёрский", "Астероиды, руда и одиночество на дальних поясах.", new[] { "Нашёл плазму, но она шевелится.", "Не трогай." }),
        ("БОТ", "Ботаника", "Грядки, амброзия и подозрительные грибы.", new[] { "Кто полил грибы из шланга?", "Это был не шланг." }),
        ("АТМ", "Атмосферика", "Газы, фильтры, разгерметизации и ремонт труб.", new[] { "В техтоннеле пахнет фороном.", "Это норма." }),
        ("ТЕХ", "Технологии", "Робототехника, ИИ, борги и законы роботов.", new[] { "ИИ снова притворяется, что не слышит.", "Он просто занят." }),
        ("РАЗВ", "Разведка", "Донесения, аванпосты и странные сигналы из сектора.", new[] { "Слышали сигнал с заброшенной станции?", "Лучше не отвечай." }),
    };

    private readonly Dictionary<string, ThreadNode> _boardRoots = new();
    private readonly Dictionary<string, int> _nextThreadId = new();
    // Which thread each cartridge currently displays, used for targeted broadcasts.
    private readonly Dictionary<EntityUid, (string Board, int Thread)> _viewerThread = new();

    public override void Initialize()
    {
        base.Initialize();
        InitializeDefaultThreads();
        LoadForum();

        _cfg.OnValueChanged(CCVars.DiscordStarWayWebhook, url => _webhookUrl = url, true);
        SubscribeLocalEvent<StarWayCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
        SubscribeLocalEvent<StarWayCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
    }

    private void InitializeDefaultThreads()
    {
        foreach (var (code, name, description, posts) in BoardSeeds)
        {
            var root = new ThreadNode
            {
                Id = 0,
                Title = $"/{code.ToLowerInvariant()}/ - {name}",
                Description = description,
            };

            for (var i = 0; i < posts.Length; i++)
                root.Posts.Add(new StarWayPostInfo(i + 1, "Аноним", posts[i], "24/10/26"));

            _boardRoots[code] = root;
            _nextThreadId[code] = 1;
        }
    }

    private void OnUiReady(EntityUid uid, StarWayCartridgeComponent component, CartridgeUiReadyEvent args)
    {
        // Nothing to push: the client requests a specific thread itself.
    }

    private void OnUiMessage(EntityUid uid, StarWayCartridgeComponent component, CartridgeMessageEvent args)
    {
        if (args is not StarWayUiMessageEvent message)
            return;

        if (!_playerManager.TryGetSessionByEntity(args.Actor, out var session))
            return;

        var loaderEntity = GetEntity(args.LoaderUid);

        // The search index spans all boards, so it is handled before the per-board lookup.
        if (message.Action == StarWayUiAction.ListThreads)
        {
            var entries = new List<StarWayThreadEntry>();
            foreach (var (boardCode, rootNode) in _boardRoots)
                CollectThreads(boardCode, rootNode, entries, isSubthread: false);

            _cartridgeLoaderSystem?.UpdateCartridgeUiState(loaderEntity, new StarWayThreadIndexState(entries), session);
            return;
        }

        if (!_boardRoots.TryGetValue(message.BoardCode, out var root))
            return;

        _viewerThread[uid] = (message.BoardCode, message.ThreadId);

        switch (message.Action)
        {
            case StarWayUiAction.OpenThread:
            case StarWayUiAction.Refresh:
            {
                var node = FindThread(root, message.ThreadId) ?? root;
                _viewerThread[uid] = (message.BoardCode, node.Id);
                _cartridgeLoaderSystem?.UpdateCartridgeUiState(
                    loaderEntity, BuildState(message.BoardCode, root, node), session);
                break;
            }

            case StarWayUiAction.PostMessage:
            {
                var node = FindThread(root, message.ThreadId) ?? root;
                if (string.IsNullOrWhiteSpace(message.Text))
                    return;

                var author = string.IsNullOrWhiteSpace(message.Author) ? "Аноним" : message.Author;
                var timestamp = DateTimeOffset.UtcNow.ToString("dd/MM/yy");
                var newPost = new StarWayPostInfo(node.Posts.Count + 1, author, message.Text.Trim(), timestamp, session.Name);
                node.Posts.Add(newPost);

                _adminLogger.Add(LogType.Action, LogImpact.Low,
                    $"{ToPrettyString(args.Actor)} posted to StarWay/{message.BoardCode}/{node.Id}: '{message.Text}'");

                // Admins see every post with the real account name in admin chat, and the
                // same info is mirrored to Discord when a webhook is configured.
                _chat.SendAdminAnnouncement(
                    $"[StarWay /{message.BoardCode.ToLowerInvariant()}/] {session.Name}: {newPost.Text}");
                SendForumWebhook(session.Name, message.BoardCode, node.Title, newPost.Text);

                BroadcastThread(message.BoardCode, root, node.Id);
                SaveForum();
                break;
            }

            case StarWayUiAction.CreateSubthread:
            {
                var parent = FindThread(root, message.ThreadId) ?? root;

                // One level of nesting only: a subthread cannot get its own subthread.
                if (parent != root)
                    return;

                var title = message.Title?.Trim();
                if (string.IsNullOrWhiteSpace(title))
                    return;

                if (title.Length > 60)
                    title = title[..60];

                var child = new ThreadNode
                {
                    Id = _nextThreadId[message.BoardCode]++,
                    Title = title,
                    Description = $"Подтред в {parent.Title}",
                };
                parent.Children.Add(child);

                _adminLogger.Add(LogType.Action, LogImpact.Low,
                    $"{ToPrettyString(args.Actor)} created StarWay subthread '{title}' in {message.BoardCode}/{parent.Id}");

                // Drop the creator into the fresh subthread.
                _viewerThread[uid] = (message.BoardCode, child.Id);
                _cartridgeLoaderSystem?.UpdateCartridgeUiState(
                    loaderEntity, BuildState(message.BoardCode, root, child), session);

                // Everybody else still on the parent gets the new link.
                BroadcastThread(message.BoardCode, root, parent.Id);
                SaveForum();
                break;
            }

            case StarWayUiAction.DeletePost:
            {
                if (!_adminManager.IsAdmin(session))
                    return;

                var node = FindThread(root, message.ThreadId) ?? root;
                var index = node.Posts.FindIndex(p => p.Number == message.PostNumber);
                if (index < 0)
                    return;

                var removed = node.Posts[index];
                node.Posts.RemoveAt(index);

                _adminLogger.Add(LogType.Action, LogImpact.Medium,
                    $"{ToPrettyString(args.Actor)} deleted StarWay post #{removed.Number} in {message.BoardCode}/{node.Id}");

                BroadcastThread(message.BoardCode, root, node.Id);
                SaveForum();
                break;
            }
        }
    }

    private static void CollectThreads(string boardCode, ThreadNode node, List<StarWayThreadEntry> entries, bool isSubthread)
    {
        var haystack = $"{boardCode} {node.Title} {node.Description}";
        foreach (var post in node.Posts)
            haystack += $" {post.Author} {post.Text}";

        entries.Add(new StarWayThreadEntry(boardCode, node.Id, node.Title, node.Description, node.Posts.Count, isSubthread,
            haystack.ToLowerInvariant().Replace("ё", "е")));

        foreach (var child in node.Children)
            CollectThreads(boardCode, child, entries, true);
    }

    private StarWayUiState BuildState(string boardCode, ThreadNode root, ThreadNode node)
    {
        var parentId = FindParent(root, node.Id)?.Id ?? -1;
        var posts = new List<StarWayPostInfo>(node.Posts);
        var children = new List<StarWaySubthreadInfo>(node.Children.Count);
        foreach (var child in node.Children)
            children.Add(new StarWaySubthreadInfo(child.Id, child.Title, child.Posts.Count));

        return new StarWayUiState(boardCode, node.Id, parentId, node.Title, node.Description, posts, children);
    }

    private void BroadcastThread(string boardCode, ThreadNode root, int threadId)
    {
        var node = FindThread(root, threadId);
        if (node == null)
            return;

        var state = BuildState(boardCode, root, node);
        var query = EntityQueryEnumerator<CartridgeLoaderComponent>();
        while (query.MoveNext(out var loaderUid, out var loader))
        {
            if (loader.ActiveProgram is not { } program)
                continue;

            if (!HasComp<StarWayCartridgeComponent>(program))
                continue;

            if (!_viewerThread.TryGetValue(program, out var view))
                continue;

            if (view.Board != boardCode || view.Thread != threadId)
                continue;

            _cartridgeLoaderSystem.UpdateCartridgeUiState(loaderUid, state);
        }
    }

    private async void SendForumWebhook(string author, string board, string threadTitle, string text)
    {
        if (string.IsNullOrWhiteSpace(_webhookUrl))
            return;

        try
        {
            var webhookData = await _discord.GetWebhook(_webhookUrl);
            if (webhookData == null)
                return;

            var payload = new WebhookPayload
            {
                Username = "StarWay",
                Content = $"**{author}** · /{board.ToLowerInvariant()}/ {threadTitle}\n{text}",
            };

            await _discord.CreateMessage(webhookData.Value.ToIdentifier(), payload);
        }
        catch (Exception e)
        {
            Sawmill.Error($"Failed to send StarWay webhook: {e}");
        }
    }

    private void LoadForum()
    {
        try
        {
            if (!_res.UserData.Exists(ForumSavePath))
                return;

            using var stream = _res.UserData.Open(ForumSavePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var saved = JsonSerializer.Deserialize<Dictionary<string, ThreadDto>>(json);

            if (saved == null)
                return;

            foreach (var (code, dto) in saved)
            {
                var node = ToNode(dto);
                _boardRoots[code] = node;
                _nextThreadId[code] = MaxThreadId(node) + 1;
            }

            Sawmill.Info($"Loaded StarWay forum from disk ({saved.Count} boards).");
        }
        catch (Exception e)
        {
            Sawmill.Error($"Failed to load StarWay forum: {e}");
        }
    }

    private void SaveForum()
    {
        try
        {
            var dto = new Dictionary<string, ThreadDto>();
            foreach (var (code, root) in _boardRoots)
                dto[code] = ToDto(root);

            using var stream = _res.UserData.Open(ForumSavePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            writer.Write(JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception e)
        {
            Sawmill.Error($"Failed to save StarWay forum: {e}");
        }
    }

    private static ThreadDto ToDto(ThreadNode node)
    {
        var dto = new ThreadDto
        {
            Id = node.Id,
            Title = node.Title,
            Description = node.Description,
        };

        foreach (var post in node.Posts)
        {
            dto.Posts.Add(new PostDto
            {
                Number = post.Number,
                Author = post.Author,
                Text = post.Text,
                Timestamp = post.Timestamp,
                RealAuthor = post.RealAuthor,
            });
        }

        foreach (var child in node.Children)
            dto.Children.Add(ToDto(child));

        return dto;
    }

    private static ThreadNode ToNode(ThreadDto dto)
    {
        var node = new ThreadNode
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
        };

        foreach (var post in dto.Posts)
            node.Posts.Add(new StarWayPostInfo(post.Number, post.Author, post.Text, post.Timestamp, post.RealAuthor));

        foreach (var child in dto.Children)
            node.Children.Add(ToNode(child));

        return node;
    }

    private static int MaxThreadId(ThreadNode node)
    {
        var max = node.Id;
        foreach (var child in node.Children)
            max = Math.Max(max, MaxThreadId(child));

        return max;
    }

    private static ThreadNode? FindThread(ThreadNode node, int id)
    {
        if (node.Id == id)
            return node;

        foreach (var child in node.Children)
        {
            var found = FindThread(child, id);
            if (found != null)
                return found;
        }

        return null;
    }

    private static ThreadNode? FindParent(ThreadNode node, int childId)
    {
        foreach (var child in node.Children)
        {
            if (child.Id == childId)
                return node;

            var found = FindParent(child, childId);
            if (found != null)
                return found;
        }

        return null;
    }
}
