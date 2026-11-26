using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Discord;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.DiscordStatus;

public sealed partial class DiscordOnlineStatusSystem : EntitySystem
{
    [Dependency] private DiscordWebhook _webhook = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private ILogManager _log = default!;
    [Dependency] private IGameTiming _timing = default!;

    private static readonly TimeSpan MinimumInterval = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan ChangeCooldown = TimeSpan.FromSeconds(5);
    private const string StatePath = "discord_online_status.json";

    private ISawmill _sawmill = default!;
    private WebhookIdentifier? _identifier;
    private ulong? _messageId;
    private bool _busy;
    private bool _shuttingDown;
    private int _lastPlayers = -1;
    private TimeSpan _nextUpdate;
    private TimeSpan _nextAttempt;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _log.GetSawmill("discord.online");
        Subs.CVar(_cfg, CCVars.DiscordOnlineWebhook, OnWebhookChanged, true);
    }

    public override void Shutdown()
    {
        MarkOffline();
        base.Shutdown();
    }

    /// <summary>
    ///     Edits the Discord online message to the offline state. Called on server shutdown.
    /// </summary>
    public void MarkOffline()
    {
        if (_shuttingDown)
            return;

        _shuttingDown = true;

        if (_identifier is not { } identifier || _messageId is not { } messageId)
            return;

        try
        {
            var task = Task.Run(async () =>
            {
                using var response = await _webhook.EditMessage(identifier, messageId, BuildOfflinePayload());
            });

            if (!task.Wait(TimeSpan.FromSeconds(5)))
                _sawmill.Warning("Timed out while marking Discord online status as offline");
        }
        catch (Exception e)
        {
            _sawmill.Warning($"Failed to mark Discord online status as offline: {e.Message}");
        }
    }

    private void OnWebhookChanged(string url)
    {
        _identifier = ParseWebhook(url);
        _messageId = null;
        _lastPlayers = -1;
        _nextUpdate = TimeSpan.Zero;

        if (_identifier is not { } identifier)
            return;

        if (LoadState() is { } state && state.WebhookId == identifier.Id)
            _messageId = state.MessageId;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_identifier == null || _busy || _shuttingDown)
            return;

        var now = _timing.CurTime;
        var count = _players.PlayerCount;
        var interval = TimeSpan.FromSeconds(Math.Max(MinimumInterval.TotalSeconds,
            _cfg.GetCVar(CCVars.DiscordOnlineUpdateSeconds)));

        var periodic = now >= _nextUpdate;
        var changed = count != _lastPlayers && now >= _nextAttempt;
        if (!periodic && !changed)
            return;

        if (periodic)
            _nextUpdate = now + interval;

        _nextAttempt = now + ChangeCooldown;
        _lastPlayers = count;
        _busy = true;
        _ = UpdateMessageAsync(BuildPayload(count));
    }

    private async Task UpdateMessageAsync(WebhookPayload payload)
    {
        try
        {
            if (_identifier is not { } identifier)
                return;

            if (_messageId is { } messageId)
            {
                var edit = await _webhook.EditMessage(identifier, messageId, payload);
                if (edit.StatusCode != HttpStatusCode.NotFound)
                {
                    edit.Dispose();
                    return;
                }

                edit.Dispose();
                _messageId = null;
                SaveState(null);
            }

            await CreateMessageAsync(identifier, payload);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to update Discord online status: {e}");
        }
        finally
        {
            if (_shuttingDown)
                await MarkOfflineAfterUpdateAsync();

            _busy = false;
        }
    }

    private async Task MarkOfflineAfterUpdateAsync()
    {
        if (_identifier is not { } identifier || _messageId is not { } messageId)
            return;

        using var response = await _webhook.EditMessage(identifier, messageId, BuildOfflinePayload());
    }

    private async Task CreateMessageAsync(WebhookIdentifier identifier, WebhookPayload payload)
    {
        var response = await _webhook.CreateMessage(identifier, payload);
        if (!response.IsSuccessStatusCode)
        {
            response.Dispose();
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        response.Dispose();

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("id", out var idProp)
            || !ulong.TryParse(idProp.GetString(), out var id))
            return;

        _messageId = id;
        SaveState(id);
    }

    private WebhookPayload BuildPayload(int count)
    {
        var name = _cfg.GetCVar(CCVars.DiscordOnlineServerName);
        if (string.IsNullOrWhiteSpace(name))
            name = _cfg.GetCVar(Robust.Shared.CVars.GameHostName);

        var max = _cfg.GetCVar(CCVars.DiscordOnlineMaxPlayers);
        var online = max > 0 ? $"**{count}** / {max}" : $"**{count}**";
        var invite = _cfg.GetCVar(CCVars.DiscordOnlineInvite);

        var embed = new WebhookEmbed
        {
            Title = name,
            Url = string.IsNullOrWhiteSpace(invite) ? null : invite,
            Description = $"🟢 Онлайн: {online}",
            Color = count > 0 ? 0x57F287 : 0x95A5A6,
            Footer = new WebhookEmbedFooter { Text = "Обновляется автоматически" },
            Timestamp = DateTimeOffset.UtcNow,
            Fields = new List<WebhookEmbedField>()
        };

        return new WebhookPayload
        {
            Embeds = new List<WebhookEmbed> { embed },
            AllowedMentions = new WebhookMentions()
        };
    }

    private WebhookPayload BuildOfflinePayload()
    {
        var name = _cfg.GetCVar(CCVars.DiscordOnlineServerName);
        if (string.IsNullOrWhiteSpace(name))
            name = _cfg.GetCVar(Robust.Shared.CVars.GameHostName);

        var invite = _cfg.GetCVar(CCVars.DiscordOnlineInvite);

        var embed = new WebhookEmbed
        {
            Title = name,
            Url = string.IsNullOrWhiteSpace(invite) ? null : invite,
            Description = "🔴 Сервер оффлайн",
            Color = 0xED4245,
            Footer = new WebhookEmbedFooter { Text = "Сервер выключен" },
            Timestamp = DateTimeOffset.UtcNow,
            Fields = new List<WebhookEmbedField>()
        };

        return new WebhookPayload
        {
            Embeds = new List<WebhookEmbed> { embed },
            AllowedMentions = new WebhookMentions()
        };
    }

    private static WebhookIdentifier? ParseWebhook(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var trimmed = url.Split('?')[0].TrimEnd('/');
        var parts = trimmed.Split('/');
        if (parts.Length < 2)
            return null;

        var id = parts[^2];
        var token = parts[^1];
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
            return null;

        return new WebhookIdentifier(id, token);
    }

    private static StatusState? LoadState()
    {
        try
        {
            if (!File.Exists(StatePath))
                return null;

            using var doc = JsonDocument.Parse(File.ReadAllText(StatePath));
            var root = doc.RootElement;
            if (!root.TryGetProperty("webhookId", out var webhookId)
                || !root.TryGetProperty("messageId", out var messageId)
                || !ulong.TryParse(messageId.GetString(), out var id))
                return null;

            var webhook = webhookId.GetString();
            return string.IsNullOrWhiteSpace(webhook) ? null : new StatusState(webhook, id);
        }
        catch
        {
            return null;
        }
    }

    private void SaveState(ulong? messageId)
    {
        try
        {
            var state = new { webhookId = _identifier?.Id ?? "", messageId = messageId?.ToString() };
            File.WriteAllText(StatePath, JsonSerializer.Serialize(state));
        }
        catch (Exception e)
        {
            _sawmill.Warning($"Failed to save Discord online status state: {e.Message}");
        }
    }

    private readonly record struct StatusState(string WebhookId, ulong MessageId);
}
