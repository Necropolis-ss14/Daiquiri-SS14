using Content.Shared.Actions;
using Content.Shared.VoiceMask;

namespace Content.Shared._Daiquiri.Harpy;

public sealed partial class HarpySingerSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HarpySingerComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<HarpySingerComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, HarpySingerComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, ref component.MidiAction, component.MidiActionId);

        if (TryComp<VoiceMaskComponent>(uid, out var voiceMask))
            _actionsSystem.AddAction(uid, ref voiceMask.ActionEntity, voiceMask.Action, uid);
    }

    private void OnShutdown(EntityUid uid, HarpySingerComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.MidiAction);

        if (TryComp<VoiceMaskComponent>(uid, out var voiceMask))
            _actionsSystem.RemoveAction(uid, voiceMask.ActionEntity);
    }
}
