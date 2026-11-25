using Content.Shared._Daiquiri.Xeno.Plasma;
using Content.Shared._Starlight.Antags.Xeno.Components;
using Content.Shared.Placeable;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Xeno.Plasma;

public sealed partial class PlasmaSystem : SharedPlasmaSystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlasmaGainModifierComponent, ItemPlacedEvent>(OnItemPlaced);
        SubscribeLocalEvent<PlasmaGainModifierComponent, ItemRemovedEvent>(OnItemRemoved);
    }

    private void OnItemPlaced(Entity<PlasmaGainModifierComponent> ent, ref ItemPlacedEvent args)
    {
        if (!TryComp<XenoComponent>(args.OtherEntity, out var xeno) || xeno.OnWeed)
            return;

        xeno.OnWeed = true;
    }

    private void OnItemRemoved(Entity<PlasmaGainModifierComponent> ent, ref ItemRemovedEvent args)
    {
        if (!TryComp<XenoComponent>(args.OtherEntity, out var xeno) || !xeno.OnWeed)
            return;

        foreach (var contact in _physics.GetContactingEntities(args.OtherEntity))
        {
            if (contact == ent.Owner || !HasComp<PlasmaGainModifierComponent>(contact))
                continue;

            return;
        }

        xeno.OnWeed = false;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var time = _timing.CurTime;
        var query = EntityQueryEnumerator<PlasmaVesselComponent>();
        while (query.MoveNext(out var uid, out var vessel))
        {
            if (time < vessel.NextPointsAt)
                continue;

            vessel.NextPointsAt = time + TimeSpan.FromSeconds(1);

            var amount = vessel.PlasmaPerSecondOffWeed;
            if (TryComp<XenoComponent>(uid, out var xeno) && xeno.OnWeed)
                amount = vessel.PlasmaPerSecondOnWeed;

            ChangePlasmaAmount(uid, amount, vessel);
        }
    }
}
