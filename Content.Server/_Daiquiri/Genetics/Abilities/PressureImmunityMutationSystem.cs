using Content.Server.Atmos.Components;
using Content.Shared._Daiquiri.Genetics.Abilities;
using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Server._Daiquiri.Genetics.Abilities;

public sealed partial class PressureImmunityMutationSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PressureImmunityMutationComponent, MutationAddedEvent>(OnAdded);
        SubscribeLocalEvent<PressureImmunityMutationComponent, MutationRemovedEvent>(OnRemoved);
    }

    private void OnAdded(Entity<PressureImmunityMutationComponent> ent, ref MutationAddedEvent args)
    {
        EnsureComp<PressureImmunityComponent>(args.Target);
    }

    private void OnRemoved(Entity<PressureImmunityMutationComponent> ent, ref MutationRemovedEvent args)
    {
        RemComp<PressureImmunityComponent>(args.Target);
    }
}