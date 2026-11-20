using Content.Server.Administration.Managers;
using Content.Server.Discord;
using Content.Server.Players.RateLimiting;
using Content.Server._NullLink.PlayerData;
using Content.Shared.Administration.Logs;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Players.RateLimiting;
using Content.Shared._Starlight.CCVar;
using Content.Shared._Starlight.MHelp;
using Content.Shared._NullLink;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Content.Server._Starlight;

[UsedImplicitly]
public sealed partial class MentorSystem : SharedMentorSystem
{
    private const string RateLimitKey = "MentorHelp";

    // Max embed description length is 4096, according to https://discord.com/developers/docs/resources/channel#embed-object-embed-limits
    private const ushort DescriptionMax = 4000;
    private const ushort MessageLengthCap = 3000;
    private const string TooLongText = "... **(too long)**";
    private const int DiscordRelayColor = 0x00FFFF;

    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private ISharedNullLinkPlayerRolesReqManager _playerRoles = default!;
    [Dependency] private INullLinkPlayerManager _nullLinkPlayers = default!;
    [Dependency] private IAdminManager _adminManager = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private PlayerRateLimitManager _rateLimit = default!;
    [Dependency] private SharedTransformSystem _xform = default!;
    [Dependency] private ISharedAdminLogManager _alog = default!;
    [Dependency] private DiscordWebhook _discord = default!;
    [Dependency] private IConfigurationManager _config = default!;

    private readonly Dictionary<Guid, MentorTicket> _tickets = [];
    private readonly Dictionary<Guid, DiscordTicketRelay> _ticketRelays = [];
    private ISawmill _sawmill = default!;

    private WebhookIdentifier? _mhelpWebhook;

    private readonly Dictionary<NetUserId, (TimeSpan Timestamp, bool Typing)> _typingUpdateTimestamps = [];

    [GeneratedRegex(@"^https://discord\.com/api/webhooks/(\d+)/((?!.*/).*)$")]
    private static partial Regex DiscordRegex();

    [GeneratedRegex(@"\\?\[\/?[^\]]*\]")]
    private static partial Regex MarkupRegex();

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("MHELP");

        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;

        SubscribeNetworkEvent<MHelpTypingRequest>(OnClientTypingUpdated);
        SubscribeNetworkEvent<MHelpCloseTicket>(OnCloseTicket);
        SubscribeNetworkEvent<MhelpTptoTicket>(OnTptoTicket);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(_ =>
        {
            _tickets.Clear();
            _ticketRelays.Clear();
            _typingUpdateTimestamps.Clear();
        });

        Subs.CVar(_config, StarlightCCVars.MHelpWebhook, OnWebhookChanged, true);

        _rateLimit.Register(
            RateLimitKey,
            new RateLimitRegistration(StarlightCCVars.MhelpRateLimitPeriod,
                StarlightCCVars.MhelpRateLimitCount,
                PlayerRateLimitedAction)
            );
    }
    private void PlayerRateLimitedAction(ICommonSession obj)
        => RaiseNetworkEvent(
            new MHelpTextMessage
            {
                Sender = obj.UserId,
                Ticket = null,
                Text = Loc.GetString("mentor-system-rate-limited"),
                PlaySound = false
            },
            obj.Channel);
    protected override void OnMentoringTextMessage(MHelpTextMessage message, EntitySessionEventArgs eventArgs)
    {
        MentorTicket? ticket = null;
        var senderSession = eventArgs.SenderSession;
        var adminData = _adminManager.GetAdminData(senderSession);

        var senderIsAdmin = adminData?.HasFlag(AdminFlags.Adminhelp) ?? false;
        var senderIsMentor = _playerRoles.IsMentor(senderSession);
        if (!senderIsAdmin && !senderIsMentor && _rateLimit.CountAction(senderSession, RateLimitKey) != RateLimitStatus.Allowed)
            return;

        if (message.Ticket is not Guid ticketId)
        {
            ticketId = Guid.NewGuid();

            ticket = new MentorTicket
            {
                Id = ticketId,
                Title = $"{senderSession.Name}[{ticketId.ToString()[..4]}]",
                Creator = senderSession.UserId
            };

            _tickets.Add(ticketId, ticket);

            var @event = new MHelpTextMessage()
            {
                Sender = senderSession.UserId,
                Ticket = ticketId,
                Title = ticket.Title,
                Text = Loc.GetString("mentor-system-ticket-created"),
                PlaySound = false
            };
            RaiseNetworkEvent(@event, senderSession.Channel);

            RelayToDiscord(ticketId, ticket.Title, @event.Text);
        }
        if (ticket is null && !_tickets.TryGetValue(ticketId, out ticket))
            return;
        if (ticket.Creator != senderSession.UserId
        && ((ticket.Mentor is null && !senderIsMentor) || (ticket.Mentor != senderSession.UserId))
        && !(senderIsAdmin || senderIsMentor))
            return;
        var escapedText = FormattedMessage.EscapeText(message.Text);

        var text = senderIsAdmin ? $"{(message.PlaySound ? "" : "(S) ")}[color=#9B59B6][bold]\\[{Loc.GetString("mentor-help-tag-admin")}\\][/bold] {senderSession.Name}[/color]: {escapedText}"
                : senderIsMentor ? $"{(message.PlaySound ? "" : "(S) ")}[color=#00ffff][bold]\\[{Loc.GetString("mentor-help-tag-mentor")}\\][/bold] {senderSession.Name}[/color]: {escapedText}"
                                 : $"{(message.PlaySound ? "" : "(S) ")}{senderSession.Name}: {escapedText}";

        var msg = new MHelpTextMessage
        {
            Sender = senderSession.UserId,
            Ticket = ticketId,
            Text = text,
            Title = ticket.Title,
            PlaySound = true
        };
        _sawmill.Info($"mhelp message: {text}");
        RelayToDiscord(ticketId, ticket.Title, text);
        // Commented out on request of Tess.
        // if (ticket.Mentor is null && (senderIsMentor || senderIsAdmin))
        // {
        //     ticket.Mentor = senderSession.UserId;
        //     var @event = new MHelpTextMessage()
        //     {
        //         Sender = senderSession.UserId,
        //         Ticket = ticketId,
        //         Title = ticket.Title,
        //         Text = Loc.GetString("mentor-system-ticket-claimed", ("name", senderSession.Name)),
        //         PlaySound = false,
        //         TicketClosed = true
        //     };
        //     var nonparticipantsRecipients = _nullLinkPlayers.Mentors
        //         .Except([_playerManager.GetSessionById(ticket.Creator), _playerManager.GetSessionById(ticket.Mentor.Value)])
        //         .Except(_adminManager.ActiveAdmins);
        //     foreach (var channel in nonparticipantsRecipients)
        //         RaiseNetworkEvent(@event, channel);
        //
        //     @event.TicketClosed = false;
        //     var participantsRecipients = _adminManager.ActiveAdmins
        //         .Concat([_playerManager.GetSessionById(ticket.Creator), _playerManager.GetSessionById(ticket.Mentor.Value)])
        //         .Distinct();
        //     foreach (var channel in participantsRecipients)
        //         RaiseNetworkEvent(@event, channel);
        // }

        var recipients = _adminManager.ActiveAdmins
            .Concat([_playerManager.GetSessionById(ticket.Creator)])
            .Concat(ticket.Mentor is not null
                ? [_playerManager.GetSessionById(ticket.Mentor.Value)]
                : _nullLinkPlayers.Mentors)
            .Distinct();

        foreach (var channel in recipients)
            RaiseNetworkEvent(msg, channel);
    }
    private void OnCloseTicket(MHelpCloseTicket message, EntitySessionEventArgs eventArgs)
    {
        MentorTicket? ticket = null;
        var senderSession = eventArgs.SenderSession;
        var adminData = _adminManager.GetAdminData(senderSession);

        var senderIsAdmin = adminData?.HasFlag(AdminFlags.Adminhelp) ?? false;
        var senderIsMentor = _playerRoles.IsMentor(senderSession);
        if (!senderIsAdmin && !senderIsMentor && _rateLimit.CountAction(senderSession, RateLimitKey) != RateLimitStatus.Allowed)
            return;
        if (message.Ticket is not Guid ticketId)
            return;
        if (ticket is null && !_tickets.TryGetValue(ticketId, out ticket))
            return;
        if (ticket.IsClosed)
            return;
        if (ticket.Creator != senderSession.UserId
            && (ticket.Mentor != senderSession.UserId)
            && !(senderIsAdmin || senderIsMentor))
            return;
        ticket.IsClosed = true;
        var msg = new MHelpTextMessage()
        {
            Sender = senderSession.UserId,
            Ticket = ticketId,
            Text = Loc.GetString("mentor-system-ticket-closed"),
            TicketClosed = true,
            PlaySound = false
        };

        _sawmill.Info($"mhelp ticket {ticketId} closed");
        RelayToDiscord(ticketId, ticket.Title, msg.Text);

        var recipients = _adminManager.ActiveAdmins
            .Concat([_playerManager.GetSessionById(ticket.Creator)])
            .Concat(ticket.Mentor is not null
                ? [_playerManager.GetSessionById(ticket.Mentor.Value)]
                : _nullLinkPlayers.Mentors)
            .Distinct();

        foreach (var channel in recipients)
            RaiseNetworkEvent(msg, channel);
    }

    private void OnTptoTicket(MhelpTptoTicket message, EntitySessionEventArgs eventArgs)
    {
        MentorTicket? ticket = null;
        var senderSession = eventArgs.SenderSession;
        var adminData = _adminManager.GetAdminData(senderSession);

        var senderIsAdmin = adminData?.HasFlag(AdminFlags.Adminhelp) ?? false;
        var senderIsMentor = _playerRoles.IsMentor(senderSession);
        if (!(senderIsAdmin || senderIsMentor)) //only admins/mentors can use mtpto
            return;
        if (message.Ticket is not Guid ticketId)
            return;
        if (ticket is null && !_tickets.TryGetValue(ticketId, out ticket))
            return;
        var mentorEnt = senderSession.AttachedEntity;
        if (!HasComp<GhostComponent>(mentorEnt))
            return;

        var playerSession = _playerManager.GetSessionById(ticket.Creator);

        if (playerSession.AttachedEntity.HasValue)
            _xform.SetCoordinates(mentorEnt.Value, Transform(playerSession.AttachedEntity.Value).Coordinates);

        _alog.Add(LogType.AdminCommands, LogImpact.Low, $"mentor {senderSession} tpto'd ticket {ticketId} aka {playerSession}");
    }

    private void OnClientTypingUpdated(MHelpTypingRequest msg, EntitySessionEventArgs args)
    {
        if (_typingUpdateTimestamps.TryGetValue(args.SenderSession.UserId, out var tuple) &&
            tuple.Typing == msg.Typing &&
            tuple.Timestamp + TimeSpan.FromSeconds(1) > _timing.RealTime)
            return;

        _typingUpdateTimestamps[args.SenderSession.UserId] = (_timing.RealTime, msg.Typing);

        MentorTicket? ticket = null;
        var senderSession = args.SenderSession;

        if (ticket is null && !_tickets.TryGetValue(msg.Ticket, out ticket))
            return;

        var recipients = _adminManager.ActiveAdmins
            .Concat([_playerManager.GetSessionById(ticket.Creator)])
            .Concat(ticket.Mentor is not null
                ? [_playerManager.GetSessionById(ticket.Mentor.Value)]
                : _nullLinkPlayers.Mentors)
            .Except([senderSession])
            .Distinct();

        var update = new MHelpTypingUpdated
        {
            Typing = msg.Typing,
            Ticket = msg.Ticket,
            PlayerName = senderSession.Name
        };

        foreach (var channel in recipients)
            RaiseNetworkEvent(update, channel);
    }

    private async void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        var message = e.NewStatus switch
        {
            SessionStatus.Connected => Loc.GetString("mentor-system-player-reconnecting"),
            SessionStatus.Disconnected => Loc.GetString("mentor-system-player-disconnecting"),
            _ => null
        };

        if (message == null) return;
        var tickets = _tickets.Values.Where(t => t.Creator == e.Session.UserId || t.Mentor == e.Session.UserId);
        foreach (var ticket in tickets)
        {
            var recipients = _adminManager.ActiveAdmins
                 .Concat(ticket.Mentor is not null
                     ? [_playerManager.GetSessionById(ticket.Mentor.Value)]
                     : _nullLinkPlayers.Mentors)
                 .Except([e.Session])
                 .Distinct();

            var msg = new MHelpTextMessage
            {
                Sender = e.Session.UserId,
                Ticket = ticket.Id,
                Text = message,
                PlaySound = false
            };

            foreach (var channel in recipients)
                RaiseNetworkEvent(msg, channel);

            RelayToDiscord(ticket.Id, ticket.Title, msg.Text);
        }
    }

    private async void OnWebhookChanged(string url)
    {
        RaiseNetworkEvent(new MentoringDiscordRelayUpdated(!string.IsNullOrWhiteSpace(url)));

        if (string.IsNullOrWhiteSpace(url))
        {
            _mhelpWebhook = null;
            return;
        }

        var match = DiscordRegex().Match(url);

        if (!match.Success)
        {
            Log.Warning("MHelp webhook URL does not appear to be valid.");
            _mhelpWebhook = null;
            return;
        }

        var identifier = new WebhookIdentifier(match.Groups[1].Value, match.Groups[2].Value);

        if (await _discord.GetWebhook(url) is null)
        {
            _sawmill.Error("Could not get MHelp webhook data, the URL is probably invalid.");
            _mhelpWebhook = null;
            return;
        }

        _mhelpWebhook = identifier;
    }

    private async void RelayToDiscord(Guid ticketId, string title, string text)
    {
        if (_mhelpWebhook is not { } identifier)
            return;

        var clean = StripMarkup(text);

        if (string.IsNullOrWhiteSpace(clean))
            return;

        if (!_ticketRelays.TryGetValue(ticketId, out var relay))
        {
            relay = new DiscordTicketRelay();
            _ticketRelays[ticketId] = relay;
        }

        if (clean.Length > MessageLengthCap)
            clean = clean[..(MessageLengthCap - TooLongText.Length)] + TooLongText;

        if (relay.MessageId == null || relay.Description.Length + clean.Length + 1 > DescriptionMax)
        {
            relay.MessageId = null;
            relay.Description = string.Empty;
        }

        relay.Description += $"\n{clean}";

        var payload = GenerateMHelpPayload(relay.Description, title);

        try
        {
            if (relay.MessageId == null)
            {
                var response = await _discord.CreateMessage(identifier, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _sawmill.Error($"Failed to create MHelp discord message: {response.StatusCode}\n{content}");
                    _ticketRelays.Remove(ticketId);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();

                if (ulong.TryParse(JsonNode.Parse(json)?["id"]?.ToString(), out var messageId))
                    relay.MessageId = messageId;
            }
            else
            {
                await _discord.EditMessage(identifier, relay.MessageId.Value, payload);
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"Error while relaying MHelp message to discord.\n{e}");
        }
    }

    private WebhookPayload GenerateMHelpPayload(string messages, string title)
    {
        return new WebhookPayload
        {
            Username = Loc.GetString("mentor-discord-webhook-username"),
            Embeds = new List<WebhookEmbed>
            {
                new()
                {
                    Title = title,
                    Description = messages,
                    Color = DiscordRelayColor,
                    Footer = new WebhookEmbedFooter
                    {
                        Text = Loc.GetString("mentor-discord-webhook-footer")
                    }
                }
            }
        };
    }

    private static string StripMarkup(string text)
    {
        return MarkupRegex().Replace(text, string.Empty);
    }
}

public sealed class MentorTicket
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public NetUserId Creator { get; set; }
    public NetUserId? Mentor { get; set; }
    public bool IsClosed { get; set; }
}

public sealed class DiscordTicketRelay
{
    public ulong? MessageId;
    public string Description = string.Empty;
}
