using Content.Shared._Lavaland.Megafauna.Components;
using Content.Shared._Lavaland.Megafauna.Systems;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Lavaland.Megafauna.Selectors;

/// <summary>
/// Performs an action and if required, tries to get target positions
/// from <see cref="MegafaunaAiTargetingComponent"/>.
/// </summary>
public sealed partial class PerformActionSelector : MegafaunaSelector
{
    [DataField]
    public EntProtoId ActionId;

    protected override float InvokeImplementation(MegafaunaCalculationBaseArgs args)
    {
        var entMan = args.EntityManager;
        var actionSys = entMan.System<SharedActionsSystem>();
        var megafaunaSys = entMan.System<MegafaunaSystem>();

        if (!megafaunaSys.TryGetActionById(args.Entity, ActionId, out var action))
        {
            DebugTools.Assert($"{entMan.ToPrettyString(args.Entity)}'s AI failed to get an action with ID {ActionId}!");
            return FailDelay;
        }

        var ev = megafaunaSys.GetPerformEvent(args.Entity, action.Owner);

        if (ev == null)
        {
            DebugTools.Assert($"{entMan.ToPrettyString(args.Entity)}'s AI failed to build an event for action {ActionId}!");
            return FailDelay;
        }

        actionSys.PerformAction(args.Entity, action, ev, predicted: false);

        if (!ev.Handled)
        {
            DebugTools.Assert($"{entMan.ToPrettyString(args.Entity)}'s AI failed to perform action {entMan.ToPrettyString(action.Owner)} with ID {ActionId}!");
            return FailDelay;
        }

        return DelaySelector.Get(args);
    }
}
