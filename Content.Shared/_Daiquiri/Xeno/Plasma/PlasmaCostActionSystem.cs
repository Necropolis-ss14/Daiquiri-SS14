using Content.Shared.Actions;
using Content.Shared.Actions.Events;

namespace Content.Shared._Daiquiri.Xeno.Plasma;

public sealed partial class PlasmaCostActionSystem : EntitySystem
{
    [Dependency] private readonly SharedPlasmaSystem _plasma = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlasmaCostActionComponent, ActionAttemptEvent>(OnActionAttempt);
        SubscribeLocalEvent<PlasmaCostActionComponent, ActionPerformedEvent>(OnActionPerformed);
    }

    private void OnActionAttempt(Entity<PlasmaCostActionComponent> ent, ref ActionAttemptEvent args)
    {
        if (!_plasma.HasPlasma(args.User, ent.Comp.PlasmaCost))
            args.Cancelled = true;
    }

    private void OnActionPerformed(Entity<PlasmaCostActionComponent> ent, ref ActionPerformedEvent args)
    {
        if (ent.Comp.ShouldChangePlasma)
            _plasma.ChangePlasmaAmount(args.Performer, -ent.Comp.PlasmaCost);
    }
}
