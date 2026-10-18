using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Players.RateLimiting;
using Content.Shared._Starlight.AIHelp;
using Content.Shared._Starlight.CCVar;
using Content.Shared.Administration;
using Content.Shared.GameTicking;
using Content.Shared.Players.RateLimiting;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Starlight.AIHelp;

/// <summary>
///     Server side of the AI help window. Keeps the conversation history and forwards it to an
///     OpenAI-compatible chat completions API (Groq by default) configured through the aihelp.* CVars.
/// </summary>
[UsedImplicitly]
public sealed partial class AIHelpSystem : SharedAIHelpSystem
{
    private const string RateLimitKey = "AIHelp";

    [Dependency] private IConfigurationManager _config = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IAdminManager _adminManager = default!;
    [Dependency] private PlayerRateLimitManager _rateLimit = default!;

    private readonly Dictionary<NetUserId, List<AIHelpChatMessage>> _history = new();
    private ISawmill _sawmill = default!;

    private const int AdminLogLimit = 200;
    private const int AdminLogReplyPreview = 4000;

    private readonly List<AIHelpRequestLogEntry> _log = new();

    private readonly Dictionary<int, CancellationTokenSource> _pendingRequests = new();

    private readonly HashSet<NetUserId> _muted = new();

    private int _nextLogId = 1;

    private HttpClient? _http;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _cachedAccessToken;
    private DateTime _accessTokenExpires = DateTime.MinValue;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("AIHELP");

        _rateLimit.Register(RateLimitKey,
            new RateLimitRegistration(StarlightCCVars.AIHelpRateLimitPeriod,
                StarlightCCVars.AIHelpRateLimitCount,
                PlayerRateLimitedAction));

        SubscribeLocalEvent<RoundRestartCleanupEvent>(_ => _history.Clear());
        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    private void PlayerRateLimitedAction(ICommonSession session)
        => RaiseNetworkEvent(new AIHelpResponseMessage
        {
            Text = Loc.GetString("ai-help-rate-limited"),
            IsError = true
        }, session.Channel);

    private HttpClient GetHttpClient()
    {
        if (_http != null)
            return _http;

        var handler = new HttpClientHandler();
        if (_config.GetCVar(StarlightCCVars.AIHelpAllowInvalidCert))
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            _sawmill.Warning("[AIHelp] aihelp.allow_invalid_cert=true — проверка TLS-сертификатов API отключена.");
        }

        _http = new HttpClient(handler);
        _sawmill.Debug("[AIHelp] Создан HttpClient для запросов к API ИИ.");
        return _http;
    }

    /// <summary>Скрывает секреты в логах — видны только начало и конец строки.</summary>
    private static string Mask(string value)
        => value.Length <= 8 ? "***" : $"{value[..4]}…{value[^4..]} (длина {value.Length})";

    /// <summary>Обрезает длинные строки для логов.</summary>
    private static string Short(string value, int max)
        => value.Length <= max ? value : $"{value[..max]}…(+{value.Length - max} симв.)";

    private static string FormatUsage(AIHelpUsage? usage)
        => usage == null ? string.Empty : $" (токены: {usage.PromptTokens} промпт + {usage.CompletionTokens} ответ = {usage.TotalTokens})";

    private bool IsGigaChat()
        => _config.GetCVar(StarlightCCVars.AIHelpAuthType).Equals("gigachat", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     GigaChat uses OAuth2: the authorization key is exchanged for a short-lived access token
    ///     (30 minutes by default), which is then used as a Bearer token.
    /// </summary>
    private async Task<string?> GetAccessTokenAsync(string authKey, CancellationToken cancel)
    {
        if (_cachedAccessToken != null && DateTime.UtcNow < _accessTokenExpires)
        {
            _sawmill.Debug($"[AIHelp] OAuth: берём кэшированный токен ({Mask(_cachedAccessToken)}, действует до {_accessTokenExpires:HH:mm:ss} UTC).");
            return _cachedAccessToken;
        }

        await _tokenLock.WaitAsync(cancel);
        try
        {
            if (_cachedAccessToken != null && DateTime.UtcNow < _accessTokenExpires)
                return _cachedAccessToken;

            var url = _config.GetCVar(StarlightCCVars.AIHelpOAuthUrl);
            var scope = _config.GetCVar(StarlightCCVars.AIHelpOAuthScope);
            if (string.IsNullOrWhiteSpace(url))
            {
                _sawmill.Error("[AIHelp] OAuth: не задан aihelp.oauth_url (он пуст по умолчанию, задай его в server_config.toml).");
                return null;
            }

            var rquid = Guid.NewGuid().ToString();

            _sawmill.Info($"[AIHelp] OAuth: POST {url} (scope={scope}, RqUID={rquid}, Authorization=Basic {Mask(authKey)}).");

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authKey);
            request.Headers.Add("RqUID", rquid);
            request.Headers.Accept.ParseAdd("application/json");
            request.Headers.UserAgent.ParseAdd("LightLine-AIHelp/1.0");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "scope", scope }
            });

            var watch = Stopwatch.StartNew();
            using var response = await GetHttpClient().SendAsync(request, cancel);
            var body = await response.Content.ReadAsStringAsync(cancel);
            watch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _sawmill.Error($"[AIHelp] OAuth: HTTP {(int) response.StatusCode} {response.ReasonPhrase} за {watch.ElapsedMilliseconds} мс, тело: {Short(body, 2000)}");
                return null;
            }

            var parsed = JsonSerializer.Deserialize<GigaChatAuthResponse>(body);
            if (parsed == null || string.IsNullOrWhiteSpace(parsed.AccessToken))
            {
                _sawmill.Error($"[AIHelp] OAuth: неожиданный ответ за {watch.ElapsedMilliseconds} мс: {Short(body, 2000)}");
                return null;
            }

            _cachedAccessToken = parsed.AccessToken;
            _accessTokenExpires = parsed.ExpiresAt > 0
                ? DateTimeOffset.FromUnixTimeMilliseconds(parsed.ExpiresAt).UtcDateTime - TimeSpan.FromSeconds(60)
                : DateTime.UtcNow.AddMinutes(25);

            _sawmill.Info($"[AIHelp] OAuth: токен получен за {watch.ElapsedMilliseconds} мс ({Mask(parsed.AccessToken)}), запросы работают до {_accessTokenExpires:HH:mm:ss} UTC.");
            return _cachedAccessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    protected override void OnAIHelpRequest(AIHelpRequestMessage message, EntitySessionEventArgs eventArgs)
    {
        var session = eventArgs.SenderSession;

        var text = message.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return;

        _sawmill.Info($"[AIHelp] {session.Name} ({session.UserId}): запрос «{Short(text, 300)}»");

        if (!_config.GetCVar(StarlightCCVars.AIHelpEnabled))
        {
            _sawmill.Info($"[AIHelp] {session.Name}: aihelp.enabled=false — запрос отклонён.");
            SendError(session, "ai-help-disabled");
            return;
        }

        var apiKey = _config.GetCVar(StarlightCCVars.AIHelpApiKey);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            SendError(session, "ai-help-not-configured");
            _sawmill.Warning($"[AIHelp] {session.Name}: запрос при пустом aihelp.api_key — настрой сервер (server_config.toml).");
            return;
        }

        // У API-cvar нет значений по умолчанию: адрес и модель должны быть заданы в конфиге сервера.
        if (string.IsNullOrWhiteSpace(_config.GetCVar(StarlightCCVars.AIHelpApiUrl)) ||
            string.IsNullOrWhiteSpace(_config.GetCVar(StarlightCCVars.AIHelpModel)))
        {
            SendError(session, "ai-help-not-configured");
            _sawmill.Warning($"[AIHelp] {session.Name}: не заданы aihelp.api_url или aihelp.model — настрой сервер (server_config.toml).");
            return;
        }

        var isAdmin = _adminManager.GetAdminData(session)?.HasFlag(AdminFlags.Adminhelp) ?? false;
        if (!isAdmin && _rateLimit.CountAction(session, RateLimitKey) != RateLimitStatus.Allowed)
        {
            _sawmill.Info($"[AIHelp] {session.Name}: сработал лимит запросов (aihelp.rate_limit_*) — запрос отклонён.");
            return;
        }

        var maxLength = _config.GetCVar(StarlightCCVars.AIHelpMaxMessageLength);
        if (maxLength > 0 && text.Length > maxLength)
        {
            _sawmill.Debug($"[AIHelp] {session.Name}: сообщение обрезано с {text.Length} до {maxLength} симв.");
            text = text[..maxLength];
        }

        if (_muted.Contains(session.UserId))
        {
            var blocked = AddLogEntry(session, text, AIHelpRequestStatus.Blocked);
            blocked.Reply = Loc.GetString("ai-help-admin-muted");
            SendError(session, "ai-help-admin-muted");
            BroadcastAdminLog();
            _sawmill.Info($"[AIHelp] {session.Name}: доступ к ИИ закрыт администратором — запрос отклонён.");
            return;
        }

        var entry = AddLogEntry(session, text, AIHelpRequestStatus.Pending);
        BroadcastAdminLog();

        _ = RespondAsync(session, text, apiKey, entry);
    }

    protected override void OnAIHelpReset(AIHelpResetMessage message, EntitySessionEventArgs eventArgs)
        => _history.Remove(eventArgs.SenderSession.UserId);

    private void SendError(ICommonSession session, string locKey)
        => RaiseNetworkEvent(new AIHelpResponseMessage
        {
            Text = Loc.GetString(locKey),
            IsError = true
        }, session.Channel);

    protected override void OnAIHelpAdminLogRequest(AIHelpAdminLogRequestMessage message, EntitySessionEventArgs eventArgs)
    {
        var session = eventArgs.SenderSession;
        if (!CanOversee(session))
        {
            _sawmill.Warning($"[AIHelp] {session.Name}: запрос журнала ИИ без флага Adminhelp — отклонено.");
            return;
        }

        RaiseNetworkEvent(BuildAdminLogMessage(), session.Channel);
    }

    protected override void OnAIHelpAdminCancelRequest(AIHelpAdminCancelRequestMessage message, EntitySessionEventArgs eventArgs)
    {
        var session = eventArgs.SenderSession;
        if (!CanOversee(session))
            return;

        if (!_pendingRequests.TryGetValue(message.EntryId, out var cts))
            return;

        var entry = _log.FirstOrDefault(e => e.Id == message.EntryId);
        if (entry != null)
            entry.Status = AIHelpRequestStatus.Cancelled;

        cts.Cancel();
        _sawmill.Info($"[AIHelp] {session.Name}: прервал запрос #{message.EntryId}.");
        BroadcastAdminLog();
    }

    protected override void OnAIHelpAdminResetRequest(AIHelpAdminResetRequestMessage message, EntitySessionEventArgs eventArgs)
    {
        var session = eventArgs.SenderSession;
        if (!CanOversee(session))
            return;

        _history.Remove(message.UserId);
        _sawmill.Info($"[AIHelp] {session.Name}: сбросил контекст ИИ игрока {message.UserId}.");
    }

    protected override void OnAIHelpAdminMuteRequest(AIHelpAdminMuteRequestMessage message, EntitySessionEventArgs eventArgs)
    {
        var session = eventArgs.SenderSession;
        if (!CanOversee(session))
            return;

        if (message.Muted)
            _muted.Add(message.UserId);
        else
            _muted.Remove(message.UserId);

        foreach (var entry in _log.Where(e => e.UserId == message.UserId))
            entry.Muted = message.Muted;

        _sawmill.Info($"[AIHelp] {session.Name}: {(message.Muted ? "закрыл" : "открыл")} доступ к ИИ игроку {message.UserId}.");
        BroadcastAdminLog();
    }

    private bool CanOversee(ICommonSession session)
        => _adminManager.GetAdminData(session)?.HasFlag(AdminFlags.Adminhelp) ?? false;

    private AIHelpRequestLogEntry AddLogEntry(ICommonSession session, string text, AIHelpRequestStatus status)
    {
        var entry = new AIHelpRequestLogEntry
        {
            Id = _nextLogId++,
            UserId = session.UserId,
            PlayerName = session.Name,
            Time = DateTime.Now.ToString("HH:mm:ss"),
            Text = text,
            Status = status,
            Muted = _muted.Contains(session.UserId),
        };

        _log.Add(entry);
        if (_log.Count > AdminLogLimit)
            _log.RemoveAt(0);

        return entry;
    }

    private AIHelpAdminLogMessage BuildAdminLogMessage()
        => new()
        {
            Entries = new List<AIHelpRequestLogEntry>(_log),
            Muted = _muted.ToList(),
        };

    private void BroadcastAdminLog()
    {
        var payload = BuildAdminLogMessage();
        foreach (var session in _playerManager.Sessions)
        {
            if (!CanOversee(session))
                continue;

            RaiseNetworkEvent(payload, session.Channel);
        }
    }

    private async Task RespondAsync(ICommonSession session, string userText, string apiKey, AIHelpRequestLogEntry entry)
    {
        var channel = session.Channel;

        var timeout = _config.GetCVar(StarlightCCVars.AIHelpTimeoutSeconds);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(1, timeout)));
        _pendingRequests[entry.Id] = cts;

        var history = GetHistory(session.UserId);
        history.Add(new AIHelpChatMessage { Role = "user", Content = userText });
        TrimHistory(history);

        // Let the client show the typing indicator while we wait for the API.
        RaiseNetworkEvent(new AIHelpResponseMessage
        {
            Text = string.Empty,
            Typing = true
        }, channel);

        var reply = string.Empty;
        var isError = false;
        var watch = Stopwatch.StartNew();

        try
        {
            var url = _config.GetCVar(StarlightCCVars.AIHelpApiUrl);
            var model = _config.GetCVar(StarlightCCVars.AIHelpModel);
            var maxTokens = _config.GetCVar(StarlightCCVars.AIHelpMaxTokens);
            var temperature = _config.GetCVar(StarlightCCVars.AIHelpTemperature);
            var systemPrompt = _config.GetCVar(StarlightCCVars.AIHelpSystemPrompt);

            var messages = new List<AIHelpChatMessage>();
            if (!string.IsNullOrWhiteSpace(systemPrompt))
                messages.Add(new AIHelpChatMessage { Role = "system", Content = systemPrompt });
            messages.AddRange(history);

            var payload = new AIHelpApiRequest
            {
                Model = model,
                Messages = messages,
                MaxTokens = maxTokens,
                Temperature = temperature
            };

            _sawmill.Info($"[AIHelp] {session.Name}: POST {url} (model={model}, auth={(IsGigaChat() ? "gigachat" : "bearer")}, сообщений {messages.Count}, история {history.Count}, max_tokens={maxTokens}, temperature={temperature}, timeout={timeout} с).");
            if (!string.IsNullOrWhiteSpace(systemPrompt))
                _sawmill.Debug($"[AIHelp] {session.Name}: system ({systemPrompt.Length} симв.): {Short(systemPrompt, 300)}");
            foreach (var m in messages)
                _sawmill.Debug($"[AIHelp] {session.Name}: → {m.Role}: {Short(m.Content, 400)}");

            var bearer = apiKey;
            if (IsGigaChat())
            {
                var token = await GetAccessTokenAsync(apiKey, cts.Token);
                if (string.IsNullOrWhiteSpace(token))
                {
                    isError = true;
                    reply = Loc.GetString("ai-help-auth-error");
                }
                else
                {
                    bearer = token;
                }
            }

            if (!isError)
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
                // Некоторые шлюзы (Cloudflare перед Groq) отдают 403 на запросы без User-Agent.
                request.Headers.UserAgent.ParseAdd("LightLine-AIHelp/1.0");
                request.Headers.Accept.ParseAdd("application/json");

                var httpWatch = Stopwatch.StartNew();
                using var response = await GetHttpClient().SendAsync(request, cts.Token);
                var body = await response.Content.ReadAsStringAsync(cts.Token);
                httpWatch.Stop();

                _sawmill.Info($"[AIHelp] {session.Name}: HTTP {(int) response.StatusCode} {response.ReasonPhrase} за {httpWatch.ElapsedMilliseconds} мс, получено {body.Length} байт.");
                _sawmill.Debug($"[AIHelp] {session.Name}: тело ответа: {Short(body, 4000)}");

                if (!response.IsSuccessStatusCode)
                {
                    isError = true;
                    reply = Loc.GetString("ai-help-error");
                    _sawmill.Error($"[AIHelp] {session.Name}: API вернул {(int) response.StatusCode} {response.ReasonPhrase}: {Short(body, 2000)}");
                }
                else
                {
                    var parsed = JsonSerializer.Deserialize<AIHelpApiResponse>(body);
                    reply = parsed?.Choices is { Count: > 0 } choices
                        ? choices[0].Message?.Content?.Trim() ?? string.Empty
                        : string.Empty;

                    if (string.IsNullOrWhiteSpace(reply))
                    {
                        isError = true;
                        reply = Loc.GetString("ai-help-empty-response");
                        _sawmill.Warning($"[AIHelp] {session.Name}: API вернул пустой ответ, тело: {Short(body, 2000)}");
                    }
                    else
                    {
                        entry.Tokens = parsed?.Usage?.TotalTokens ?? 0;
                        _sawmill.Info($"[AIHelp] {session.Name}: ответ модели {parsed?.Model ?? model}{FormatUsage(parsed?.Usage)}, {reply.Length} симв.: {Short(reply, 500)}");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            isError = true;
            if (entry.Status == AIHelpRequestStatus.Cancelled)
            {
                reply = Loc.GetString("ai-help-admin-cancelled");
                _sawmill.Info($"[AIHelp] {session.Name}: запрос #{entry.Id} прерван администратором через {watch.ElapsedMilliseconds} мс.");
            }
            else
            {
                reply = Loc.GetString("ai-help-timeout");
                _sawmill.Warning($"[AIHelp] {session.Name}: таймаут через {watch.ElapsedMilliseconds} мс (aihelp.timeout_seconds).");
            }
        }
        catch (Exception e)
        {
            isError = true;
            reply = Loc.GetString("ai-help-error");
            _sawmill.Error($"[AIHelp] {session.Name}: ошибка через {watch.ElapsedMilliseconds} мс: {e}");
        }

        if (!isError && !string.IsNullOrWhiteSpace(reply))
        {
            history.Add(new AIHelpChatMessage { Role = "assistant", Content = reply });
            TrimHistory(history);
        }
        else if (history.Count > 0 && history[^1].Role == "user")
        {
            // Запрос не удался: убираем реплику из истории, иначе следующая попытка
            // отправит два сообщения подряд от пользователя (GigaChat такого не принимает).
            history.RemoveAt(history.Count - 1);
            _sawmill.Debug($"[AIHelp] {session.Name}: неудачная реплика убрана из истории (осталось {history.Count}).");
        }

        _sawmill.Info($"[AIHelp] {session.Name}: готово за {watch.ElapsedMilliseconds} мс — {(isError ? $"ОШИБКА: {Short(reply, 200)}" : $"ответ {reply.Length} симв.")}");

        _pendingRequests.Remove(entry.Id);
        if (entry.Status != AIHelpRequestStatus.Cancelled)
            entry.Status = isError ? AIHelpRequestStatus.Failed : AIHelpRequestStatus.Answered;
        entry.Reply = reply.Length > AdminLogReplyPreview ? reply[..AdminLogReplyPreview] : reply;
        BroadcastAdminLog();

        RaiseNetworkEvent(new AIHelpResponseMessage
        {
            Text = reply,
            IsError = isError
        }, channel);
    }

    private List<AIHelpChatMessage> GetHistory(NetUserId userId)
    {
        if (!_history.TryGetValue(userId, out var history))
            _history[userId] = history = new List<AIHelpChatMessage>();

        return history;
    }

    private void TrimHistory(List<AIHelpChatMessage> history)
    {
        var exchanges = Math.Max(0, _config.GetCVar(StarlightCCVars.AIHelpHistoryLength));
        var max = exchanges * 2;
        if (max <= 0)
        {
            history.Clear();
            return;
        }

        if (history.Count > max)
            history.RemoveRange(0, history.Count - max);
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        if (e.NewStatus == SessionStatus.Disconnected)
            _history.Remove(e.Session.UserId);
    }

    private sealed class AIHelpApiRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<AIHelpChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    private sealed class AIHelpChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class GigaChatAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }
    }

    private sealed class AIHelpApiResponse
    {
        [JsonPropertyName("choices")]
        public List<AIHelpChoice>? Choices { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("usage")]
        public AIHelpUsage? Usage { get; set; }
    }

    private sealed class AIHelpUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    private sealed class AIHelpChoice
    {
        [JsonPropertyName("message")]
        public AIHelpChatMessage? Message { get; set; }
    }
}
