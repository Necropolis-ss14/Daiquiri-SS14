using Robust.Shared.Configuration;

namespace Content.Shared._Daiquiri.CCVar;

[CVarDefs]
public sealed class DaiquiriCCVars
{
    /// <summary>
    ///     Whether the AI help window is available on this server.
    /// </summary>
    public static readonly CVarDef<bool> AIHelpEnabled =
        CVarDef.Create("aihelp.enabled", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    ///     OpenAI-compatible chat completions endpoint.
    ///     Пусто по умолчанию — адрес задаётся только в server_config.toml,
    ///     чтобы в репозитории не было ни адресов, ни ключей.
    /// </summary>
    public static readonly CVarDef<string> AIHelpApiUrl =
        CVarDef.Create("aihelp.api_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     API key used to authenticate against the endpoint. Never sent to clients.
    ///     Всегда пусто по умолчанию: ключ живёт только в server_config.toml.
    /// </summary>
    public static readonly CVarDef<string> AIHelpApiKey =
        CVarDef.Create("aihelp.api_key", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Authorization scheme used for the API.
    ///     "bearer"   — api_key is sent as a static Bearer token (Cloudflare Workers AI, Groq, OpenAI, Ollama, ...).
    ///     "gigachat" — api_key is the GigaChat authorization key (Basic), and an OAuth access token
    ///                  is requested from aihelp.oauth_url.
    /// </summary>
    public static readonly CVarDef<string> AIHelpAuthType =
        CVarDef.Create("aihelp.auth_type", "bearer", CVar.SERVERONLY);

    /// <summary>
    ///     OAuth token endpoint, used when aihelp.auth_type = "gigachat". Пусто по умолчанию.
    /// </summary>
    public static readonly CVarDef<string> AIHelpOAuthUrl =
        CVarDef.Create("aihelp.oauth_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     OAuth scope, used when aihelp.auth_type = "gigachat". Пусто по умолчанию.
    /// </summary>
    public static readonly CVarDef<string> AIHelpOAuthScope =
        CVarDef.Create("aihelp.oauth_scope", "", CVar.SERVERONLY);

    /// <summary>
    ///     Disables TLS certificate validation for AI API requests. Needed for GigaChat: its certificate
    ///     chain is signed by the Russian Trusted Root CA, which is usually absent from the system store.
    /// </summary>
    public static readonly CVarDef<bool> AIHelpAllowInvalidCert =
        CVarDef.Create("aihelp.allow_invalid_cert", false, CVar.SERVERONLY);

    /// <summary>
    ///     Model name to request from the API. Пусто по умолчанию — задаётся в server_config.toml.
    /// </summary>
    public static readonly CVarDef<string> AIHelpModel =
        CVarDef.Create("aihelp.model", "", CVar.SERVERONLY);

    /// <summary>
    ///     System prompt that gives the model its persona and server context.
    /// </summary>
    public static readonly CVarDef<string> AIHelpSystemPrompt =
        CVarDef.Create("aihelp.system_prompt",
            "Ты — ИИ-помощник игрового сервера Space Station 14 (LightLine). Отвечай на русском языке, кратко и по делу. Помогай игрокам разобраться в механиках игры: профессии, системы станции, антагонисты, крафт, управление. Если не знаешь ответа — честно признайся. Не выдумывай информацию о текущем раунде и не помогай нарушать правила сервера.",
            CVar.SERVERONLY);

    /// <summary>
    ///     Maximum number of tokens the model may generate in a single answer.
    /// </summary>
    public static readonly CVarDef<int> AIHelpMaxTokens =
        CVarDef.Create("aihelp.max_tokens", 1024, CVar.SERVERONLY);

    /// <summary>
    ///     Sampling temperature passed to the API.
    /// </summary>
    public static readonly CVarDef<float> AIHelpTemperature =
        CVarDef.Create("aihelp.temperature", 0.7f, CVar.SERVERONLY);

    /// <summary>
    ///     How many previous messages (user+assistant) are kept in the conversation context.
    /// </summary>
    public static readonly CVarDef<int> AIHelpHistoryLength =
        CVarDef.Create("aihelp.history_length", 10, CVar.SERVERONLY);

    /// <summary>
    ///     Request timeout in seconds.
    /// </summary>
    public static readonly CVarDef<float> AIHelpTimeoutSeconds =
        CVarDef.Create("aihelp.timeout_seconds", 30f, CVar.SERVERONLY);

    /// <summary>
    ///     Maximum length of a single player message. Longer messages are truncated.
    /// </summary>
    public static readonly CVarDef<int> AIHelpMaxMessageLength =
        CVarDef.Create("aihelp.max_message_length", 500, CVar.SERVERONLY);

    public static readonly CVarDef<float> AIHelpRateLimitPeriod =
        CVarDef.Create("aihelp.rate_limit_period", 5f, CVar.SERVERONLY);

    public static readonly CVarDef<int> AIHelpRateLimitCount =
        CVarDef.Create("aihelp.rate_limit_count", 5, CVar.SERVERONLY);

    /// <summary>
    /// Daiquiri
    /// Выбранная пластинка в лобби.
    /// </summary>
    public static readonly CVarDef<string> LobbyVinyl =
        CVarDef.Create("lobby.vinyl", "Classic", CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<string> LobbyVinylAnnounced =
        CVarDef.Create("lobby.vinyl_announced", "", CVar.CLIENTONLY | CVar.ARCHIVE);
}
