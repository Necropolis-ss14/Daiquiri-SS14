using Content.Shared.FixedPoint;
namespace Content.Shared._Daiquiri.Xeno.Plasma;

public abstract class SharedPlasmaSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlasmaVesselComponent, TransferPlasmaActionEvent>(OnPlasmaTransfer);
    }

    private void OnPlasmaTransfer(EntityUid uid, PlasmaVesselComponent component, TransferPlasmaActionEvent args)
    {
        if (args.Handled
            || !TryComp<PlasmaVesselComponent>(args.Target, out var target)
            || !ChangePlasmaAmount(uid, -args.Amount, component))
            return;

        ChangePlasmaAmount(args.Target, args.Amount, target);
        args.Handled = true;
    }

    public bool ChangePlasmaAmount(EntityUid uid, FixedPoint2 amount, PlasmaVesselComponent? component = null)
    {
        if (!Resolve(uid, ref component) || component.Plasma + amount < 0)
            return false;

        component.Plasma = FixedPoint2.Min(component.Plasma + amount, component.MaxPlasma);
        Dirty(uid, component);
        RaiseLocalEvent(uid, new PlasmaAmountChangeEvent(component.Plasma));
        return true;
    }

    public bool HasPlasma(EntityUid uid, FixedPoint2 amount)
        => TryComp<PlasmaVesselComponent>(uid, out var comp) && comp.Plasma >= amount;
}
