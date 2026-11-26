using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordRoleBridge;

public static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private static readonly object StoreLock = new();
    private static readonly Dictionary<string, LinkEntry> Store = new();
    private static readonly Dictionary<string, DateTime> Pending = new();

    private static Config _cfg = default!;
    private static HttpClient _http = default!;
    private static string? _statusMessageId;

    public static async Task Main(string[] args)
    {
        var configPath = args.Length > 0 ? args[0] : "appsettings.json";
        _cfg = LoadConfig(configPath);
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        LoadStore();

        var listener = new HttpListener();
        listener.Prefixes.Add(_cfg.Listen);
        listener.Start();
        Console.WriteLine($"DiscordRoleBridge listening on {_cfg.Listen}");

        if (_cfg.Status.Enabled)
            _ = Task.Run(StatusLoopAsync);

        while (true)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => HandleAsync(context));
        }
    }

    private static Config LoadConfig(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config not found: {path}", path);

        var cfg = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), JsonOptions)
                  ?? throw new InvalidOperationException("Failed to parse config");

        if (string.IsNullOrWhiteSpace(cfg.SharedSecret))
            throw new InvalidOperationException("SharedSecret must be set");
        if (string.IsNullOrWhiteSpace(cfg.ApiToken))
            throw new InvalidOperationException("ApiToken must be set");
        if (string.IsNullOrWhiteSpace(cfg.Discord.ClientId) || string.IsNullOrWhiteSpace(cfg.Discord.ClientSecret))
            throw new InvalidOperationException("Discord.ClientId and Discord.ClientSecret must be set");
        if (string.IsNullOrWhiteSpace(cfg.Discord.GuildId))
            throw new InvalidOperationException("Discord.GuildId must be set");
        if (string.IsNullOrWhiteSpace(cfg.PublicBaseUrl))
            cfg.PublicBaseUrl = cfg.Listen.Replace("+", "localhost").TrimEnd('/');
        cfg.PublicBaseUrl = cfg.PublicBaseUrl.TrimEnd('/');

        return cfg;
    }

    private static void LoadStore()
    {
        lock (StoreLock)
        {
            if (!File.Exists(_cfg.DataPath))
                return;

            var entries = JsonSerializer.Deserialize<List<LinkEntry>>(File.ReadAllText(_cfg.DataPath), JsonOptions);
            if (entries is null)
                return;

            foreach (var entry in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Guid) && !string.IsNullOrWhiteSpace(entry.DiscordId))
                    Store[entry.Guid.Trim().ToLowerInvariant()] = entry;
            }
        }
    }

    private static void SaveStore()
    {
        lock (StoreLock)
        {
            var temp = _cfg.DataPath + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(Store.Values.ToList(), JsonOptions));
            File.Move(temp, _cfg.DataPath, true);
        }
    }

    private static async Task HandleAsync(HttpListenerContext ctx)
    {
        try
        {
            var path = ctx.Request.Url?.AbsolutePath ?? "/";
            switch (path)
            {
                case "/link":
                    HandleLink(ctx);
                    break;
                case "/oauth":
                    HandleOAuth(ctx);
                    break;
                case "/callback":
                    await HandleCallbackAsync(ctx);
                    break;
                case "/health":
                    Respond(ctx, 200, "ok", "text/plain; charset=utf-8");
                    break;
                default:
                    if (path.StartsWith("/api/roles/", StringComparison.Ordinal))
                        await HandleApiAsync(ctx, path["/api/roles/".Length..], true);
                    else if (path.StartsWith("/api/link/", StringComparison.Ordinal))
                        await HandleApiAsync(ctx, path["/api/link/".Length..], false);
                    else
                        Respond(ctx, 404, "not found", "text/plain; charset=utf-8");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Request failed: {e}");
            Respond(ctx, 500, "internal error", "text/plain; charset=utf-8");
        }
    }

    private static string? Query(HttpListenerRequest req, string key)
        => req.QueryString[key];

    private static string SignState(string guid)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_cfg.SharedSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(guid));
        return $"{guid}.{Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    private static string? VerifyState(string? state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return null;

        var parts = state.Split('.', 2);
        if (parts.Length != 2)
            return null;

        var guid = parts[0];
        if (!Guid.TryParse(guid, out _))
            return null;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_cfg.SharedSecret));
        var expected = hmac.ComputeHash(Encoding.UTF8.GetBytes(guid));
        byte[] actual;
        try
        {
            actual = Convert.FromHexString(parts[1]);
        }
        catch (FormatException)
        {
            return null;
        }

        if (!CryptographicOperations.FixedTimeEquals(expected, actual))
            return null;

        lock (Pending)
        {
            if (Pending.Count > 10000)
            {
                var cutoff = DateTime.UtcNow - TimeSpan.FromHours(1);
                foreach (var key in Pending.Where(x => x.Value < cutoff).Select(x => x.Key).ToList())
                    Pending.Remove(key);
            }
            Pending[guid] = DateTime.UtcNow;
        }

        return guid;
    }

    private static void HandleLink(HttpListenerContext ctx)
    {
        var guid = VerifyState(Query(ctx.Request, "state"));
        if (guid is null)
        {
            Respond(ctx, 400, Page("Ошибка", "Неверная или устаревшая ссылка. Нажмите кнопку привязки в игре."));
            return;
        }

        var state = Uri.EscapeDataString(SignState(guid));
        var body = Page("Привязка Discord",
            $"""
             <h1>Привязка Discord</h1>
             <p>Привяжите Discord-аккаунт к вашему игровому аккаунту {guid}.</p>
             <p>После подтверждения роли Discord будут переданы игровому серверу.</p>
             <a class="btn" href="/oauth?state={state}">Привязать Discord</a>
             """);
        Respond(ctx, 200, body);
    }

    private static void HandleOAuth(HttpListenerContext ctx)
    {
        var guid = VerifyState(Query(ctx.Request, "state"));
        if (guid is null)
        {
            Respond(ctx, 400, Page("Ошибка", "Неверная или устаревшая ссылка."));
            return;
        }

        var scope = Uri.EscapeDataString("identify guilds guilds.members.read");
        var redirect = Uri.EscapeDataString(_cfg.PublicBaseUrl + "/callback");
        var url = $"https://discord.com/oauth2/authorize?client_id={_cfg.Discord.ClientId}" +
                  $"&redirect_uri={redirect}&response_type=code&scope={scope}" +
                  $"&state={Uri.EscapeDataString(SignState(guid))}&prompt=consent";
        Redirect(ctx, url);
    }

    private static async Task HandleCallbackAsync(HttpListenerContext ctx)
    {
        var error = Query(ctx.Request, "error");
        if (!string.IsNullOrEmpty(error))
        {
            Respond(ctx, 400, Page("Ошибка", $"Discord вернул ошибку: {WebUtility.HtmlEncode(error)}. Попробуйте ещё раз."));
            return;
        }

        var guid = VerifyState(Query(ctx.Request, "state"));
        var code = Query(ctx.Request, "code");
        if (guid is null || string.IsNullOrEmpty(code))
        {
            Respond(ctx, 400, Page("Ошибка", "Неверные параметры ответа Discord."));
            return;
        }

        var tokens = await ExchangeCodeAsync(code);
        if (tokens is null)
        {
            Respond(ctx, 502, Page("Ошибка", "Не удалось обменять код на токен. Попробуйте ещё раз."));
            return;
        }

        var user = await DiscordGetAsync("/users/@me", tokens.AccessToken);
        if (user is null || !user.Value.TryGetProperty("id", out var idProp))
        {
            Respond(ctx, 502, Page("Ошибка", "Не удалось получить Discord ID."));
            return;
        }

        var member = await DiscordGetAsync($"/users/@me/guilds/{_cfg.Discord.GuildId}/member", tokens.AccessToken);
        var roles = new List<string>();
        if (member is not null && member.Value.TryGetProperty("roles", out var rolesProp) && rolesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var role in rolesProp.EnumerateArray())
            {
                var value = role.GetString();
                if (!string.IsNullOrEmpty(value))
                    roles.Add(value);
            }
        }

        lock (StoreLock)
        {
            Store[guid] = new LinkEntry
            {
                Guid = guid,
                DiscordId = idProp.GetString() ?? "",
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                Roles = roles,
                UpdatedAt = DateTime.UtcNow
            };
        }
        SaveStore();

        lock (Pending)
            Pending.Remove(guid);

        Respond(ctx, 200, Page("Готово",
            "<h1>Готово</h1><p>Discord привязан. Роли переданы игровому серверу.</p><p>Можно вернуться в игру.</p>"));
    }

    private static async Task HandleApiAsync(HttpListenerContext ctx, string rawGuid, bool withRoles)
    {
        if (!FixedTimeEqualsString(ctx.Request.Headers["X-Api-Token"], _cfg.ApiToken))
        {
            Respond(ctx, 403, "forbidden", "text/plain; charset=utf-8");
            return;
        }

        var guid = Uri.UnescapeDataString(rawGuid).Trim().ToLowerInvariant();
        LinkEntry? entry;
        lock (StoreLock)
            Store.TryGetValue(guid, out entry);

        if (entry is null)
        {
            RespondJson(ctx, new { linked = false, discordId = "", roles = Array.Empty<string>() });
            return;
        }

        if (withRoles && DateTime.UtcNow - entry.UpdatedAt > TimeSpan.FromMinutes(_cfg.RoleTtlMinutes))
        {
            var refreshed = await RefreshEntryAsync(entry);
            if (refreshed)
                SaveStore();
        }

        RespondJson(ctx, new
        {
            linked = true,
            discordId = entry.DiscordId,
            roles = withRoles ? (IEnumerable<string>) entry.Roles : Array.Empty<string>()
        });
    }

    private static async Task<bool> RefreshEntryAsync(LinkEntry entry)
    {
        var token = entry.AccessToken;
        var member = await DiscordGetAsync($"/users/@me/guilds/{_cfg.Discord.GuildId}/member", token);

        if (member is null && !string.IsNullOrEmpty(entry.RefreshToken))
        {
            var tokens = await RefreshTokenAsync(entry.RefreshToken);
            if (tokens is not null)
            {
                entry.AccessToken = tokens.AccessToken;
                entry.RefreshToken = tokens.RefreshToken;
                token = tokens.AccessToken;
                member = await DiscordGetAsync($"/users/@me/guilds/{_cfg.Discord.GuildId}/member", token);
            }
        }

        entry.UpdatedAt = DateTime.UtcNow;
        if (member is null || !member.Value.TryGetProperty("roles", out var rolesProp) || rolesProp.ValueKind != JsonValueKind.Array)
            return true;

        var roles = new List<string>();
        foreach (var role in rolesProp.EnumerateArray())
        {
            var value = role.GetString();
            if (!string.IsNullOrEmpty(value))
                roles.Add(value);
        }
        entry.Roles = roles;
        return true;
    }

    private static async Task<TokenResponse?> ExchangeCodeAsync(string code)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"] = _cfg.Discord.ClientId,
            ["client_secret"] = _cfg.Discord.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _cfg.PublicBaseUrl + "/callback"
        };
        return await PostTokenAsync(form);
    }

    private static async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"] = _cfg.Discord.ClientId,
            ["client_secret"] = _cfg.Discord.ClientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        };
        return await PostTokenAsync(form);
    }

    private static async Task<TokenResponse?> PostTokenAsync(Dictionary<string, string> form)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await _http.PostAsync("https://discord.com/api/oauth2/token", content);
        if (!response.IsSuccessStatusCode)
            return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(json, JsonOptions);
    }

    private static async Task<JsonElement?> DiscordGetAsync(string path, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/v10" + path);
        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + token);
        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static async Task StatusLoopAsync()
    {
        var status = _cfg.Status;
        if (string.IsNullOrWhiteSpace(_cfg.Discord.BotToken) || string.IsNullOrWhiteSpace(status.ChannelId))
        {
            Console.WriteLine("Online status: disabled (Discord.BotToken or Status.ChannelId is not set)");
            return;
        }

        _statusMessageId = status.MessageId;
        if (string.IsNullOrWhiteSpace(_statusMessageId))
            _statusMessageId = LoadStatusMessageId();

        var interval = TimeSpan.FromSeconds(Math.Max(10, status.UpdateSeconds));
        Console.WriteLine($"Online status: polling {status.ServerStatusUrl} every {interval.TotalSeconds:0} s");

        while (true)
        {
            try
            {
                var data = await FetchServerStatusAsync(status.ServerStatusUrl);
                if (data is { } online)
                {
                    var payload = BuildStatusPayload(online, status);
                    if (string.IsNullOrWhiteSpace(_statusMessageId))
                    {
                        _statusMessageId = await CreateStatusMessageAsync(status.ChannelId, payload);
                        SaveStatusMessageId(_statusMessageId);
                    }
                    else if (!await EditStatusMessageAsync(status.ChannelId, _statusMessageId, payload))
                    {
                        _statusMessageId = await CreateStatusMessageAsync(status.ChannelId, payload);
                        SaveStatusMessageId(_statusMessageId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Online status update failed: {e.Message}");
            }

            await Task.Delay(interval);
        }
    }

    private static async Task<ServerOnline?> FetchServerStatusAsync(string url)
    {
        using var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
        var players = root.TryGetProperty("players", out var playersProp) && playersProp.TryGetInt32(out var count)
            ? count
            : 0;

        return new ServerOnline(string.IsNullOrWhiteSpace(name) ? null : name, players);
    }

    private static object BuildStatusPayload(ServerOnline online, StatusConfig status)
    {
        var name = string.IsNullOrWhiteSpace(status.ServerName) ? online.Name ?? "SS14" : status.ServerName;
        var players = status.MaxPlayers > 0
            ? $"**{online.Players}** / {status.MaxPlayers}"
            : $"**{online.Players}**";

        return new
        {
            content = (string?) null,
            embeds = new[]
            {
                new
                {
                    title = name,
                    url = string.IsNullOrWhiteSpace(status.InviteUrl) ? null : status.InviteUrl,
                    description = $"🟢 Онлайн: {players}",
                    color = online.Players > 0 ? 0x57F287 : 0x95A5A6,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    footer = new { text = "Обновляется автоматически" }
                }
            },
            allowed_mentions = new { parse = Array.Empty<string>() }
        };
    }

    private static async Task<string?> CreateStatusMessageAsync(string channelId, object payload)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://discord.com/api/v10/channels/{channelId}/messages");
        request.Headers.TryAddWithoutValidation("Authorization", "Bot " + _cfg.Discord.BotToken);
        request.Content = JsonContent(payload);

        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Online status: create message failed ({(int) response.StatusCode}): " +
                              await response.Content.ReadAsStringAsync());
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("id", out var id) ? id.GetString() : null;
    }

    private static async Task<bool> EditStatusMessageAsync(string channelId, string messageId, object payload)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch,
            $"https://discord.com/api/v10/channels/{channelId}/messages/{messageId}");
        request.Headers.TryAddWithoutValidation("Authorization", "Bot " + _cfg.Discord.BotToken);
        request.Content = JsonContent(payload);

        using var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return true;

        if ((int) response.StatusCode == 429)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var retry = doc.RootElement.TryGetProperty("retry_after", out var retryProp)
                ? retryProp.GetDouble()
                : 1;
            await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(retry, 1, 60)));
            return true;
        }

        if ((int) response.StatusCode == 404)
            return false;

        Console.WriteLine($"Online status: edit message failed ({(int) response.StatusCode}): " +
                          await response.Content.ReadAsStringAsync());
        return true;
    }

    private static StringContent JsonContent(object payload)
        => new(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");

    private static string? LoadStatusMessageId()
    {
        try
        {
            if (!File.Exists(_cfg.Status.StatePath))
                return null;

            var json = File.ReadAllText(_cfg.Status.StatePath);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("messageId", out var id) ? id.GetString() : null;
        }
        catch
        {
            return null;
        }
    }

    private static void SaveStatusMessageId(string? messageId)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            return;

        try
        {
            File.WriteAllText(_cfg.Status.StatePath,
                JsonSerializer.Serialize(new { messageId }, JsonOptions));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Online status: failed to save state: {e.Message}");
        }
    }

    private static bool FixedTimeEqualsString(string? a, string? b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return false;
        var left = Encoding.UTF8.GetBytes(a);
        var right = Encoding.UTF8.GetBytes(b);
        return left.Length == right.Length && CryptographicOperations.FixedTimeEquals(left, right);
    }

    private static string Page(string title, string body)
        => "<!DOCTYPE html>" +
           "<html lang=\"ru\"><head><meta charset=\"utf-8\"><title>" + title + "</title>" +
           "<style>" +
           "body { background:#1b1c20; color:#e8e8e8; font-family:sans-serif; max-width:640px; margin:80px auto; line-height:1.6; }" +
           "h1 { color:#ffd700; }" +
           ".btn { display:inline-block; margin-top:20px; padding:12px 24px; background:#5865f2; color:#fff; text-decoration:none; border-radius:6px; }" +
           "</style></head><body>" + body + "</body></html>";

    private static void Respond(HttpListenerContext ctx, int status, string body, string contentType = "text/html; charset=utf-8")
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = contentType;
        ctx.Response.ContentLength64 = bytes.Length;
        ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
        ctx.Response.Close();
    }

    private static void RespondJson(HttpListenerContext ctx, object value)
        => Respond(ctx, 200, JsonSerializer.Serialize(value, JsonOptions), "application/json; charset=utf-8");

    private static void Redirect(HttpListenerContext ctx, string url)
    {
        ctx.Response.StatusCode = 302;
        ctx.Response.RedirectLocation = url;
        ctx.Response.Close();
    }
}

public sealed class Config
{
    public string Listen { get; set; } = "http://+:8080/";
    public string PublicBaseUrl { get; set; } = "";
    public string SharedSecret { get; set; } = "";
    public string ApiToken { get; set; } = "";
    public string DataPath { get; set; } = "bridge-data.json";
    public int RoleTtlMinutes { get; set; } = 5;
    public DiscordConfig Discord { get; set; } = new();
    public StatusConfig Status { get; set; } = new();
}

public sealed class DiscordConfig
{
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string GuildId { get; set; } = "";
    public string BotToken { get; set; } = "";
}

public sealed class StatusConfig
{
    public bool Enabled { get; set; }
    public string ServerStatusUrl { get; set; } = "http://127.0.0.1:1212/status";
    public string ChannelId { get; set; } = "";
    public string MessageId { get; set; } = "";
    public int UpdateSeconds { get; set; } = 30;
    public int MaxPlayers { get; set; }
    public string ServerName { get; set; } = "";
    public string InviteUrl { get; set; } = "";
    public string StatePath { get; set; } = "online-status.json";
}

public readonly record struct ServerOnline(string? Name, int Players);

public sealed class LinkEntry
{
    public string Guid { get; set; } = "";
    public string DiscordId { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}

public sealed class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = "";
}
