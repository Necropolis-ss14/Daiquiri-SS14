using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Shared._Daiquiri.Genetics.Abilities;

public sealed partial class MaxZoomMutationSystem : EntitySystem
{
    [Dependency] private SharedContentEyeSystem _eye = default!;
    [Dependency] private EntityQuery<ContentEyeComponent> _query = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MaxZoomMutationComponent, MutationAddedEvent>(OnAdded);
        SubscribeLocalEvent<MaxZoomMutationComponent, MutationRemovedEvent>(OnRemoved);
    }

    private void OnAdded(Entity<MaxZoomMutationComponent> ent, ref MutationAddedEvent args)
    {
        if (!_query.TryComp(args.Target, out var eye))
            return;

        var maxZoom = eye.MaxZoom * ent.Comp.Modifier;
        _eye.SetMaxZoom(args.Target, maxZoom, eye);
        _eye.SetZoom(args.Target, maxZoom, eye: eye);
    }

    private void OnRemoved(Entity<MaxZoomMutationComponent> ent, ref MutationRemovedEvent args)
    {
        if (!_query.TryComp(args.Target, out var eye))
            return;

        var maxZoom = eye.MaxZoom / ent.Comp.Modifier;
        _eye.SetMaxZoom(args.Target, maxZoom, eye);
        _eye.SetZoom(args.Target, maxZoom, eye: eye);
    }
}
