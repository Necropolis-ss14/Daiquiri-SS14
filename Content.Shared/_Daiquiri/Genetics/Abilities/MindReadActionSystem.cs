// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.IdentityManagement;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared._Daiquiri.Mind;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class MindReadActionSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private MindMessagesSystem _messages = default!;
    [Dependency] private MobStateSystem _mob = default!;
    [Dependency] private SharedCombatModeSystem _combatMode = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private EntityQuery<ActorComponent> _actorQuery = default!;

    private List<string> _recent = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MindReadActionComponent, MindReadActionEvent>(OnMindRead);
    }

    private void OnMindRead(Entity<MindReadActionComponent> ent, ref MindReadActionEvent args)
    {
        var user = args.Performer;
        var target = args.Target;

        if (!_actorQuery.TryComp(user, out var actor))
            return;

        var channel = actor.PlayerSession.Channel;

        args.Handled = true;

        var identity = Identity.Name(target, EntityManager);
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
        {
            _popup.PopupEntity(Loc.GetString("MutationMindReader-popup-target-mindless", ("target", identity)), user, user);
            return;
        }

        if (_mob.IsDead(target))
        {
            _popup.PopupEntity(Loc.GetString("MutationMindReader-popup-target-dead", ("target", identity)), user, user);
            return;
        }

        if (user == target)
        {
            _popup.PopupEntity(Loc.GetString("MutationMindReader-popup-self"), user, user);
            return;
        }

        _popup.PopupEntity(Loc.GetString("MutationMindReader-popup-plunge", ("target", identity)), user, user);

        if (_net.IsClient) return;

        if (_random.Prob(ent.Comp.AlertProb))
            _popup.PopupEntity(Loc.GetString("MutationMindReader-popup-alert"), target, target, PopupType.MediumCaution);

        if (_messages.GetMessages(mindId) is {} messages)
        {
            _recent.Clear();
            for (int i = 0; i < _random.Next(ent.Comp.MaxMessages); i++)
            {
                var msg = _messages.GetMessage(messages, i);
                if (msg.Length > 0 && _random.Prob(ent.Comp.MessageChance))
                    _recent.Add(msg);
            }

            if (_recent.Count > 0)
            {
                Tell(channel, Loc.GetString("MutationMindReader-popup-messages", ("target", target)));
                foreach (var msg in _recent)
                {
                    Tell(channel, Loc.GetString("MutationMindReader-popup-message-format", ("message", msg)));
                }
            }
        }

        var combat = _combatMode.IsInCombatMode(target);
        Tell(channel, Loc.GetString("MutationMindReader-popup-combat-mode", ("target", target), ("combat", combat)));

        if (mind.CharacterName is {} name && name != identity)
            Tell(channel, Loc.GetString("MutationMindReader-popup-true-identity", ("target", target), ("name", name)), Color.Red);
    }

    private void Tell(INetChannel client, string message, Color? color = null)
    {
        var chat = new ChatMessage(ChatChannel.Local,
            message,
            message,
            NetEntity.Invalid,
            null,
            hideChat: false,
            colorOverride: color);
        _net.ServerSendMessage(new MsgChatMessage { Message = chat }, client);
    }
}
