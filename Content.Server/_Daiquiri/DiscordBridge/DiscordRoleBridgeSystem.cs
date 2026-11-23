using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Server._NullLink.PlayerData;
using Content.Shared._Daiquiri.CCVar;
using Robust.Server.Player;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Daiquiri.DiscordBridge;

public sealed partial class DiscordRoleBridgeSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private ILogManager _log = default!;
    [Dependency] private ITaskManager _tasks = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private INullLinkPlayerManager _nullLinkPlayers = default!;

    private readonly ConcurrentDictionary<Guid, ICommonSession> _sessions = new();
    private readonly HashSet<Guid> _refreshing = [];

    private ISawmill _sawmill = default!;
    private HttpClient? _http;
    private string _url = "";
    private string _token = "";
    private bool _enabled;
    private float _refreshSeconds;
    private float _accumulator;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _log.GetSawmill("discord_bridge");
        _cfg.OnValueChanged(DiscordBridgeCCVars.Enabled, OnEnabledChanged, true);
        _cfg.OnValueChanged(DiscordBridgeCCVars.Url, OnUrlChanged, true);
        _cfg.OnValueChanged(DiscordBridgeCCVars.Token, OnTokenChanged, true);
        _cfg.OnValueChanged(DiscordBridgeCCVars.RefreshSeconds, value => _refreshSeconds = value, true);
        _players.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _players.PlayerStatusChanged -= OnPlayerStatusChanged;
        _http?.Dispose();
        _http = null;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_enabled || _http is null || _refreshSeconds <= 0)
            return;

        _accumulator += frameTime;
        if (_accumulator < _refreshSeconds)
            return;

        _accumulator = 0f;
        foreach (var userId in _sessions.Keys)
            Refresh(userId);
    }

    private void OnEnabledChanged(bool value)
    {
        _enabled = value;
        UpdateClient();
    }

    private void OnUrlChanged(string value)
    {
        _url = value.Trim().TrimEnd('/');
        UpdateClient();
    }

    private void OnTokenChanged(string value)
    {
        _token = value;
        UpdateClient();
    }

    private void UpdateClient()
    {
        _http?.Dispose();
        _http = null;

        if (!_enabled || string.IsNullOrEmpty(_url) || string.IsNullOrEmpty(_token))
            return;

        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        switch (e.NewStatus)
        {
            case SessionStatus.Connected:
                _sessions[e.Session.UserId] = e.Session;
                Refresh(e.Session.UserId);
                break;
            case SessionStatus.Disconnected:
                _sessions.TryRemove(e.Session.UserId, out _);
                break;
        }
    }

    private void Refresh(Guid userId)
    {
        lock (_refreshing)
        {
            if (!_refreshing.Add(userId))
                return;
        }

        RefreshAsync(userId).ContinueWith(task =>
        {
            lock (_refreshing)
                _refreshing.Remove(userId);

            if (task.IsFaulted)
                _sawmill.Error($"Discord bridge refresh failed for {userId}: {task.Exception}");
        }, TaskScheduler.Default);
    }

    private async Task RefreshAsync(Guid userId)
    {
        var http = _http;
        if (http is null)
            return;

        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_url}/api/roles/{Uri.EscapeDataString(userId.ToString())}");
        request.Headers.TryAddWithoutValidation("X-Api-Token", _token);

        var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _sawmill.Warning($"Discord bridge returned {(int) response.StatusCode} for {userId}");
            return;
        }

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        if (!root.TryGetProperty("linked", out var linked) || !linked.GetBoolean())
        {
            if (_nullLinkPlayers.IsDiscordLinked(userId))
                _tasks.RunOnMainThread(() => _nullLinkPlayers.SetBridgeRoles(new NetUserId(userId), 0, []));
            return;
        }

        ulong discordId = 0;
        if (root.TryGetProperty("discordId", out var idProp) && ulong.TryParse(idProp.GetString(), out var parsed))
            discordId = parsed;

        var roles = new List<ulong>();
        if (root.TryGetProperty("roles", out var rolesProp) && rolesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var role in rolesProp.EnumerateArray())
            {
                if (ulong.TryParse(role.GetString(), out var roleId))
                    roles.Add(roleId);
            }
        }

        _tasks.RunOnMainThread(() => _nullLinkPlayers.SetBridgeRoles(new NetUserId(userId), discordId, roles));
    }
}
