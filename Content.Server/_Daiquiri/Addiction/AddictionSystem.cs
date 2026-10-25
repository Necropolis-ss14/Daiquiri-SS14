using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server._Starlight.Medical.HealthAnalyzer;
using Content.Shared._Daiquiri.Addiction;
using Content.Shared._Starlight.Medical.HealthAnalyzer;
using Content.Shared._Starlight.Railroading.Events;
using Content.Shared.Body.Components;
using Content.Shared.Chat;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Rejuvenate;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Addiction;

/// <summary>
/// Handles addictions (dependencies): keeps track of how long every character has been without their
/// dose and sends escalating chat messages until they take their reagent, take a cure or stay clean
/// long enough for the addiction to go away on its own.
/// </summary>
[UsedImplicitly]
public sealed partial class AddictionSystem : EntitySystem
{
    [Dependency] private IChatManager _chat = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    [Dependency] private MobStateSystem _mobState = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IPrototypeManager _prototypes = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedSolutionContainerSystem _solutions = default!;
    [Dependency] private MovementSpeedModifierSystem _speed = default!;
    [Dependency] private IGameTiming _timing = default!;

    /// <summary>
    /// How often addictions are processed.
    /// </summary>
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    private static readonly Color CureColor = Color.FromHex("#8fd6a0");

    /// <summary>
    /// Color of the addiction entries in the health analyzer.
    /// </summary>
    private static readonly Color AnalyzerColor = Color.FromHex("#c02b2b");

    /// <summary>
    /// Color for slowdown message.
    /// </summary>
    private static readonly Color SlowdownColor = Color.FromHex("#c02b2b");

    private TimeSpan _nextUpdate;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AddictionComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshSpeed);
        SubscribeLocalEvent<AddictionComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<AddictionComponent, CollectHealthAnalyzerExtensionsEvent>(OnCollectAnalyzerData);
        SubscribeLocalEvent<AddictionComponent, RailroadingReagentMetabolizedEvent>(OnReagentMetabolized);
        SubscribeLocalEvent<AddictionComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<AddictionComponent, RejuvenateEvent>(OnRejuvenate);
    }

    /// <summary>
    /// Admin rejuvenate is a full debug heal, so it also removes every addiction/dependency.
    /// </summary>
    private void OnRejuvenate(Entity<AddictionComponent> ent, ref RejuvenateEvent args)
    {
        CureAll(ent.Owner, ent.Comp);
    }

    /// <summary>
    /// Remembers which reagents were metabolized since the last intake update. A reagent that is
    /// broken down counts as having entered the body, even though it is already gone again.
    /// </summary>
    private void OnReagentMetabolized(Entity<AddictionComponent> ent, ref RailroadingReagentMetabolizedEvent args)
    {
        var reagent = args.Reagent.Reagent.Prototype.Id;
        ent.Comp.MetabolizedReagents[reagent] =
            ent.Comp.MetabolizedReagents.GetValueOrDefault(reagent) + args.Reagent.Quantity;
    }

    /// <summary>
    /// Applies the movement speed penalty of the current withdrawal phases.
    /// </summary>
    private void OnRefreshSpeed(Entity<AddictionComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        // Sum all slowdowns (additive), clamp to max 90% slowdown
        var totalSlowdown = 0f;
        foreach (var slowdown in ent.Comp.Slowdown.Values)
        {
            if (slowdown > 0f)
                totalSlowdown += slowdown;
        }

        totalSlowdown = Math.Clamp(totalSlowdown, 0f, 0.9f);

        if (totalSlowdown > 0f)
        {
            var factor = 1f - totalSlowdown;
            args.ModifySpeed(factor, factor);
        }
    }

    /// <summary>
    /// Makes sure a penalty does not outlive the component that applied it.
    /// </summary>
    private void OnShutdown(Entity<AddictionComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Slowdown.Count == 0)
            return;

        ent.Comp.Slowdown.Clear();
        _speed.RefreshMovementSpeedModifiers(ent.Owner);
    }

    /// <summary>
    /// Shows the addictions of the scanned patient in the health analyzer.
    /// </summary>
    private void OnCollectAnalyzerData(Entity<AddictionComponent> patient, ref CollectHealthAnalyzerExtensionsEvent args)
    {
        foreach (var id in patient.Comp.Addictions)
        {
            if (!_prototypes.TryIndex(id, out var addiction))
                continue;

            args.Abnormalities.Add(new HealthAnalyzerAbnormalityData
            {
                Description = Loc.GetString("starlight-health-analyzer-abnormality-addiction", ("addiction", Loc.GetString(addiction.Name))),
                Color = AnalyzerColor,
            });
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var now = _timing.CurTime;
        if (now < _nextUpdate)
            return;

        _nextUpdate = now + UpdateInterval;

        EnsureAddictionComponents();

        var query = EntityQueryEnumerator<AddictionComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            ProcessAddictions((uid, component), now);
        }
    }

    /// <summary>
    /// Makes sure every player controlled character that can absorb reagents is tracked, so that an
    /// addiction can also appear on its own once enough of a reagent has been taken.
    /// </summary>
    private void EnsureAddictionComponents()
    {
        foreach (var session in _players.Sessions)
        {
            if (session.AttachedEntity is not { } uid || !HasComp<BloodstreamComponent>(uid))
                continue;

            EnsureComp<AddictionComponent>(uid);
        }
    }

    /// <summary>
    /// Sends initial addiction messages when a round starts or character enters the map.
    /// </summary>
    private void OnMapInit(Entity<AddictionComponent> ent, ref MapInitEvent args)
    {
        var now = _timing.CurTime;

        foreach (var id in ent.Comp.Addictions)
        {
            if (!_prototypes.TryIndex(id, out var addiction))
                continue;

            // Force immediate message on round start/map entry
            ent.Comp.NextMessage.Remove(id);
            ent.Comp.LastStage.Remove(id);
            ent.Comp.NextEmote.Remove(id);

            var elapsed = TimeSpan.Zero;
            if (ent.Comp.LastDose.TryGetValue(id, out var lastDose))
            {
                elapsed = now - lastDose;
            }

            var stage = GetStage(addiction, elapsed, s => s.Messages.Count > 0);
            if (stage != null)
            {
                ent.Comp.LastStage[id] = stage.After;
                ent.Comp.NextMessage[id] = now + stage.Interval;
                // Don't send message here - wait for first Update tick when player has session
                // SendMessage(ent.Owner, Loc.GetString(_random.Pick(stage.Messages)), stage.Color);
            }

            ProcessEmote(ent, id, addiction, elapsed, now);
            ProcessSlowdown(ent, id, addiction, elapsed);
        }
    }

    /// <summary>
    /// Adds an addiction to an entity, if it does not have it already.
    /// </summary>
    public void AddAddiction(EntityUid uid, ProtoId<AddictionPrototype> addiction, AddictionComponent? component = null)
    {
        if (!Resolve(uid, ref component, false) || component.Addictions.Contains(addiction))
            return;

        component.Addictions.Add(addiction);
        component.LastDose[addiction] = _timing.CurTime;
    }

    /// <summary>
    /// Removes a single addiction from an entity.
    /// </summary>
    public void RemoveAddiction(EntityUid uid, ProtoId<AddictionPrototype> addiction, bool notify = true, AddictionComponent? component = null)
    {
        if (!Resolve(uid, ref component, false) || !component.Addictions.Remove(addiction))
            return;

        component.LastDose.Remove(addiction);
        component.NextMessage.Remove(addiction);
        component.LastStage.Remove(addiction);
        component.NextEmote.Remove(addiction);

        // A cured character has to build up their intake from scratch.
        component.Intake.Remove(addiction);
        ClearSlowdown(uid, addiction, component);

        if (!notify || !_prototypes.TryIndex(addiction, out var prototype))
            return;

        SendMessage(uid, Loc.GetString("addiction-cured-message", ("addiction", Loc.GetString(prototype.Name))), CureColor);
    }

    /// <summary>
    /// Removes every addiction of an entity, e.g. when a cure reagent is used.
    /// </summary>
    public void CureAll(EntityUid uid, AddictionComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return;

        foreach (var addiction in component.Addictions.ToArray())
        {
            RemoveAddiction(uid, addiction, component: component);
        }
    }

    private void ProcessAddictions(Entity<AddictionComponent> ent, TimeSpan now)
    {
        var blood = GetBloodstream(ent.Owner);

        // Reagents that have just been absorbed can make the character dependent on them.
        AccumulateIntake(ent, blood);

        if (ent.Comp.Addictions.Count == 0)
            return;

        // Backwards, because curing removes entries from the list.
        for (var i = ent.Comp.Addictions.Count - 1; i >= 0; i--)
        {
            var id = ent.Comp.Addictions[i];

            if (!_prototypes.TryIndex(id, out var addiction))
            {
                Log.Error($"Unknown addiction {id} on {ToPrettyString(ent.Owner)}, removing it.");
                ent.Comp.Addictions.RemoveAt(i);
                continue;
            }

            // Cure reagents remove the addiction outright.
            if (ContainsAnyReagent(blood, addiction.Cures))
            {
                RemoveAddiction(ent.Owner, id, component: ent.Comp);
                continue;
            }

            // The character took their dose: the withdrawal starts over.
            if (ContainsAnyReagent(blood, addiction.Reagents))
            {
                ent.Comp.LastDose[id] = now;
                ent.Comp.NextMessage.Remove(id);
                ent.Comp.LastStage.Remove(id);
                ent.Comp.NextEmote.Remove(id);
                ClearSlowdown(ent.Owner, id, ent.Comp);
                continue;
            }

            if (!ent.Comp.LastDose.TryGetValue(id, out var lastDose))
            {
                // First time we see this addiction: start counting from now.
                ent.Comp.LastDose[id] = now;
                continue;
            }

            var elapsed = now - lastDose;

            // Nobody can stay clean forever.
            if (addiction.CureTime > TimeSpan.Zero && elapsed >= addiction.CureTime)
            {
                RemoveAddiction(ent.Owner, id, component: ent.Comp);
                continue;
            }

            var stage = GetStage(addiction, elapsed, s => s.Messages.Count > 0);
            if (stage != null)
            {
                // A new phase of the withdrawal begins: let the character know right away.
                if (!ent.Comp.LastStage.TryGetValue(id, out var lastStage) || lastStage != stage.After)
                {
                    ent.Comp.LastStage[id] = stage.After;
                    ent.Comp.NextMessage.Remove(id);
                }

                if (!ent.Comp.NextMessage.TryGetValue(id, out var next) || now >= next)
                {
                    ent.Comp.NextMessage[id] = now + stage.Interval;
                    SendMessage(ent.Owner, Loc.GetString(_random.Pick(stage.Messages)), stage.Color);
                }
            }

            // Phases may also force an emote, for example a smoker who has not had a cigarette.
            ProcessEmote(ent, id, addiction, elapsed, now);

            // Bad withdrawal can also weigh the character down.
            ProcessSlowdown(ent, id, addiction, elapsed);

            // Bad withdrawal can also cause tremor (shaking).
            ProcessTremor(ent, id, addiction, elapsed, now);
        }
    }

    /// <summary>
    /// Applies the movement speed penalty of the most advanced phase that has one, and removes it
    /// again once no phase asks for it anymore. Also sends a message when slowdown starts/ends.
    /// </summary>
    private void ProcessSlowdown(Entity<AddictionComponent> ent, ProtoId<AddictionPrototype> id, AddictionPrototype addiction, TimeSpan elapsed)
    {
        var slowdownStage = GetStage(addiction, elapsed, s => s.Slowdown > 0f);
        var slowdown = slowdownStage?.Slowdown ?? 0f;

        var hadSlowdown = ent.Comp.Slowdown.TryGetValue(id, out var oldSlowdown) && oldSlowdown > 0f;
        var hasSlowdown = slowdown > 0f;

        if (MathHelper.CloseTo(oldSlowdown, slowdown))
            return;

        if (!hasSlowdown)
        {
            if (hadSlowdown)
            {
                ClearSlowdown(ent.Owner, id, ent.Comp);
                // Send message when slowdown ends
                SendMessage(ent.Owner, Loc.GetString("addiction-slowdown-ended-message"), CureColor);
            }
            return;
        }

        ent.Comp.Slowdown[id] = slowdown;
        _speed.RefreshMovementSpeedModifiers(ent.Owner);

        // Send message when slowdown starts (only once when it first appears)
        if (!hadSlowdown && slowdownStage != null)
        {
            SendMessage(ent.Owner, Loc.GetString("addiction-slowdown-message"), SlowdownColor);
        }
    }

    /// <summary>
    /// Drops the movement speed penalty of a single addiction, if it had one.
    /// </summary>
    private void ClearSlowdown(EntityUid uid, ProtoId<AddictionPrototype> id, AddictionComponent component)
    {
        if (component.Slowdown.Remove(id))
            _speed.RefreshMovementSpeedModifiers(uid);
    }

    /// <summary>
    /// Clears the tremor state for a single addiction.
    /// </summary>
    private void ClearTremor(EntityUid uid, ProtoId<AddictionPrototype> id, AddictionComponent component)
    {
        component.Tremor.Remove(id);
        component.NextTremor.Remove(id);
    }

    /// <summary>
    /// Forces the character to perform a tremor (shaking) emote when withdrawal gets bad enough.
    /// The most advanced phase with Tremor=true wins.
    /// </summary>
    private void ProcessTremor(Entity<AddictionComponent> ent, ProtoId<AddictionPrototype> id, AddictionPrototype addiction, TimeSpan elapsed, TimeSpan now)
    {
        var stage = GetStage(addiction, elapsed, s => s.Tremor);

        if (stage == null)
        {
            // No phase forces tremor right now.
            ClearTremor(ent.Owner, id, ent.Comp);
            return;
        }

        var hasTremor = ent.Comp.Tremor.GetValueOrDefault(id);

        if (!hasTremor)
        {
            // Tremor starts - send message
            ent.Comp.Tremor[id] = true;
            SendMessage(ent.Owner, Loc.GetString("addiction-tremor-message"), SlowdownColor);
        }

        if (ent.Comp.NextTremor.TryGetValue(id, out var next) && now < next)
            return;

        var interval = stage.TremorInterval > TimeSpan.Zero ? stage.TremorInterval : stage.Interval;
        ent.Comp.NextTremor[id] = now + interval;

        if (_mobState.IsDead(ent.Owner) || !_players.TryGetSessionByEntity(ent.Owner, out _))
            return;

        // Use configured tremor emote or default "Shiver"
        var tremorEmote = stage.TremorEmote ?? "Shiver";
        if (!_chatSystem.TryEmoteWithChat(ent.Owner, tremorEmote.Id))
            return;
    }

    /// <summary>
    /// Adds every reagent that has just entered the bloodstream to the intake of the addictions it
    /// belongs to. Once enough has been absorbed in total the addiction appears on its own.
    /// </summary>
    private void AccumulateIntake(Entity<AddictionComponent> ent, Solution? blood)
    {
        if (blood == null)
        {
            ent.Comp.MetabolizedReagents.Clear();
            return;
        }

        // Measured once per update, because one reagent may belong to several addictions.
        Dictionary<string, FixedPoint2>? absorbed = null;

        foreach (var addiction in _prototypes.EnumeratePrototypes<AddictionPrototype>())
        {
            if (addiction.Threshold <= FixedPoint2.Zero)
                continue;

            var intake = FixedPoint2.Zero;

            foreach (var reagent in addiction.Reagents)
            {
                absorbed ??= new Dictionary<string, FixedPoint2>();

                if (!absorbed.TryGetValue(reagent.Id, out var delta))
                {
                    var amount = GetReagentAmount(blood, reagent.Id);

                    // What is still in the blood plus what the body has already broken down is
                    // exactly what has entered the body since the last time we looked.
                    var metabolized = ent.Comp.MetabolizedReagents.GetValueOrDefault(reagent.Id);
                    ent.Comp.MetabolizedReagents[reagent.Id] = FixedPoint2.Zero;

                    delta = amount - ent.Comp.LastSeenReagents.GetValueOrDefault(reagent.Id) + metabolized;
                    // Intake should never decrease - only track positive absorption
                    if (delta < FixedPoint2.Zero)
                        delta = FixedPoint2.Zero;
                    ent.Comp.LastSeenReagents[reagent.Id] = amount;
                    absorbed[reagent.Id] = delta;
                }

                if (delta > FixedPoint2.Zero)
                    intake += delta;
            }

            if (intake <= FixedPoint2.Zero || ent.Comp.Addictions.Contains(addiction.ID))
                continue;

            var total = ent.Comp.Intake.GetValueOrDefault(addiction.ID) + intake;
            ent.Comp.Intake[addiction.ID] = total;

            if (total < addiction.Threshold)
                continue;

            AddAddiction(ent.Owner, addiction.ID, ent.Comp);
            SendMessage(ent.Owner, Loc.GetString("addiction-gained-message", ("addiction", Loc.GetString(addiction.Name))), CureColor);
        }
    }

    /// <summary>
    /// Forces the character to perform the emote of the most advanced phase that defines one, for
    /// example coughing once the withdrawal of a smoking addiction goes on for long enough.
    /// </summary>
    private void ProcessEmote(Entity<AddictionComponent> ent, ProtoId<AddictionPrototype> id, AddictionPrototype addiction, TimeSpan elapsed, TimeSpan now)
    {
        var stage = GetStage(addiction, elapsed, s => s.Emote != null);

        if (stage?.Emote is not { } emote)
        {
            // No phase forces an emote right now, so the next one can start right away.
            ent.Comp.NextEmote.Remove(id);
            return;
        }

        // Felinids don't cough - skip emote for them
        if (TryComp<HumanoidAppearanceComponent>(ent.Owner, out var humanoid) &&
            _prototypes.TryIndex(humanoid.Species, out SpeciesPrototype? species) &&
            species.ID == "Felinid")
        {
            ent.Comp.NextEmote.Remove(id);
            return;
        }

        if (ent.Comp.NextEmote.TryGetValue(id, out var next) && now < next)
            return;

        var interval = stage.EmoteInterval > TimeSpan.Zero ? stage.EmoteInterval : stage.Interval;
        ent.Comp.NextEmote[id] = now + interval;

        if (_mobState.IsDead(ent.Owner) || !_players.TryGetSessionByEntity(ent.Owner, out _))
            return;

        if (!_chatSystem.TryEmoteWithChat(ent.Owner, emote.Id))
            return;
    }

    /// <summary>
    /// The most advanced phase whose "after" time has passed and that satisfies the predicate.
    /// Phases are looked up separately for messages, emotes and movement penalties, so one phase
    /// can carry only some of them without silencing the others.
    /// </summary>
    private static AddictionStage? GetStage(AddictionPrototype addiction, TimeSpan elapsed, Func<AddictionStage, bool> predicate)
    {
        AddictionStage? current = null;

        foreach (var stage in addiction.Stages)
        {
            if (stage.After > elapsed || !predicate(stage))
                continue;

            if (current == null || stage.After > current.After)
                current = stage;
        }

        return current;
    }

    private static FixedPoint2 GetReagentAmount(Solution solution, string reagent)
    {
        var amount = FixedPoint2.Zero;

        foreach (var (id, quantity) in solution.Contents)
        {
            if (id.Prototype.Id == reagent)
                amount += quantity;
        }

        return amount;
    }

    private static bool ContainsAnyReagent(Solution? solution, List<ProtoId<ReagentPrototype>> reagents)
    {
        if (solution == null || reagents.Count == 0)
            return false;

        foreach (var reagent in reagents)
        {
            if (solution.ContainsPrototype(reagent.Id))
                return true;
        }

        return false;
    }

    private Solution? GetBloodstream(EntityUid uid)
    {
        if (!TryComp<BloodstreamComponent>(uid, out var bloodstream))
            return null;

        if (!_solutions.ResolveSolution(uid, bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out var solution))
            return null;

        return solution;
    }

    /// <summary>
    /// Sends a private message to the character. Only player controlled, living characters are nagged.
    /// </summary>
    private void SendMessage(EntityUid uid, string message, Color color)
    {
        if (_mobState.IsDead(uid))
            return;

        if (!_players.TryGetSessionByEntity(uid, out var session))
            return;

        var wrapped = $"[italic]{message}[/italic]";
        _chat.ChatMessageToOne(ChatChannel.Server, message, wrapped, EntityUid.Invalid, false, session.Channel, color);
    }
}
