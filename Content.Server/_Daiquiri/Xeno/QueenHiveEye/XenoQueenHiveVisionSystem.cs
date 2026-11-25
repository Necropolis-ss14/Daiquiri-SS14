using Content.Shared._Daiquiri.Xeno.QueenHiveEye;
using Content.Shared._Daiquiri.Xeno.QueenHiveEye.UI;
using Content.Shared._Starlight.Antags.Xeno.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Server._Daiquiri.Xeno.QueenHiveEye;

public sealed partial class XenoQueenHiveVisionSystem : EntitySystem
{
    [Dependency] private SharedEyeSystem _eye = default!;
    [Dependency] private MobStateSystem _mobState = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoQueenHiveVisionEvent>(OnVisionAction);
        Subs.BuiEvents<XenoQueenHiveVisionComponent>(XenoQueenHiveUiKey.Key,
            subs => subs.Event<XenoQueenHiveChosenBuiMsg>(OnXenoChosen));
    }

    private void OnVisionAction(XenoQueenHiveVisionEvent args)
    {
        var queen = args.Performer;

        TryComp<ActorComponent>(queen, out var actor);

        if (TryComp<XenoQueenHiveVisionComponent>(queen, out var vision))
        {
            Stop(queen, vision, actor?.PlayerSession);
            args.Handled = true;
            return;
        }

        vision = EnsureComp<XenoQueenHiveVisionComponent>(queen);
        vision.Eye = SpawnAtPosition("XenoQueenEye", Transform(queen).Coordinates);

        _eye.SetTarget(queen, vision.Eye);
        _eye.SetDrawFov(queen, false);

        if (actor != null)
            _ui.OpenUi(queen, XenoQueenHiveUiKey.Key, actor.PlayerSession);

        UpdateUi(queen, vision);
        args.Handled = true;
    }

    private void OnXenoChosen(Entity<XenoQueenHiveVisionComponent> queen, ref XenoQueenHiveChosenBuiMsg args)
    {
        var target = GetEntity(args.Target);
        if (target.IsValid() && queen.Comp.Eye is { } eye)
            Transform(eye).Coordinates = Transform(target).Coordinates;
    }

    private void UpdateUi(EntityUid queen, XenoQueenHiveVisionComponent vision)
    {
        var entries = new List<XenoCameraEntry>();

        var query = EntityQueryEnumerator<XenoComponent, MobStateComponent>();
        while (query.MoveNext(out var uid, out _, out var mobState))
        {
            if (uid == queen || !_mobState.IsAlive(uid, mobState))
                continue;

            entries.Add(new XenoCameraEntry(GetNetEntity(uid), Name(uid)));
        }

        _ui.SetUiState(queen, XenoQueenHiveUiKey.Key, new XenoQueenHiveBuiState { Entries = entries });
    }

    private void Stop(EntityUid queen, XenoQueenHiveVisionComponent vision, ICommonSession? session)
    {
        _eye.SetTarget(queen, null);
        _eye.SetDrawFov(queen, true);

        if (vision.Eye is { } eye)
            QueueDel(eye);

        if (session != null)
            _ui.CloseUi(queen, XenoQueenHiveUiKey.Key, session);

        RemCompDeferred<XenoQueenHiveVisionComponent>(queen);
    }
}
