using System.Numerics;
using Content.Shared._Lavaland.Megafauna.Components;
using Content.Shared._Lavaland.Megafauna.Events;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Lavaland.Megafauna.Systems;

public sealed partial class MegafaunaSystem
{
    public void StartupMegafauna(Entity<MegafaunaAiComponent> ent)
    {
        RaiseLocalEvent(ent, new MegafaunaStartupEvent());
        ent.Comp.Active = true;
    }

    public void ShutdownMegafauna(Entity<MegafaunaAiComponent> ent)
    {
        RaiseLocalEvent(ent, new MegafaunaShutdownEvent());
        ent.Comp.Active = false;
    }

    public void KillMegafauna(Entity<MegafaunaAiComponent> ent)
    {
        RaiseLocalEvent(ent, new MegafaunaKilledEvent());
        ent.Comp.Active = false;
    }

    /// <summary>
    /// Helper method that constructs an action event for megafauna AI to use an action.
    /// </summary>
    public BaseActionEvent? GetPerformEvent(EntityUid boss, EntityUid action)
    {
        var targetingComp = CompOrNull<MegafaunaAiTargetingComponent>(boss);

        if (targetingComp != null)
        {
            if (targetingComp.TargetEnt is { } targetEnt && TerminatingOrDeleted(targetEnt))
                targetingComp.TargetEnt = null;

            if (targetingComp.TargetCoords is { } targetCoords && TerminatingOrDeleted(targetCoords.EntityId))
                targetingComp.TargetCoords = null;
        }

        var actions = EntityManager.System<SharedActionsSystem>();
        var ev = actions.GetEvent(action);

        if (ev is EntityTargetActionEvent entityEv && targetingComp?.TargetEnt is { } target)
            entityEv.Target = target;
        else if (ev is WorldTargetActionEvent worldEv && targetingComp?.TargetCoords is { } coords)
        {
            worldEv.Target = coords;
            worldEv.Entity = targetingComp.TargetEnt;
        }

        return ev;
    }

    public bool TryGetActionById(EntityUid performer, EntProtoId id, out Entity<ActionComponent> action)
    {
        action = default;
        var actions = EntityManager.System<SharedActionsSystem>();

        foreach (var current in actions.GetActions(performer))
        {
            if (MetaData(current.Owner).EntityPrototype?.ID != id.Id)
                continue;

            action = current;
            return true;
        }

        return false;
    }

    public void PickRandomPosition(MegafaunaCalculationBaseArgs args, float radius)
    {
        // TODO add an option to not pick any obstructed coordinates

        var uid = args.Entity;
        var mapId = Transform(uid).MapID;

        var randomVector = new Vector2(args.Random.NextFloat(-radius, radius), args.Random.NextFloat(-radius, radius));
        var position = _xform.GetWorldPosition(uid) + randomVector;
        var newMapCoords = new MapCoordinates(position, mapId);
        var coords = _xform.ToCoordinates(newMapCoords);

        var comp = EnsureComp<MegafaunaAiTargetingComponent>(args.Entity);
        comp.TargetEnt = null;
        comp.TargetCoords = coords;
    }
}
