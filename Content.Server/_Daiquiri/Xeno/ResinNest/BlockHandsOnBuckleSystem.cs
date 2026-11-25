using Content.Shared._Daiquiri.Xeno.ResinNest;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.VirtualItem;

namespace Content.Server._Daiquiri.Xeno.ResinNest;

public sealed partial class BlockHandsOnBuckleSystem : EntitySystem
{
    [Dependency] private SharedVirtualItemSystem _virtualItem = default!;
    [Dependency] private SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlockHandsOnBuckleComponent, StrappedEvent>(OnBuckled);
        SubscribeLocalEvent<BlockHandsOnBuckleComponent, UnstrappedEvent>(OnUnstrapped);
        SubscribeLocalEvent<BuckleComponent, AttackAttemptEvent>(OnCanAttack);
    }

    private void OnBuckled(Entity<BlockHandsOnBuckleComponent> nest, ref StrappedEvent args)
    {
        var victim = args.Buckle.Owner;
        foreach (var hand in _hands.EnumerateHands(victim))
        {
            _hands.TryDrop(victim, hand);
            _virtualItem.TrySpawnVirtualItemInHand(nest.Owner, victim, true);
            if (_hands.TryGetHeldItem(victim, hand, out var held) && held != null)
                EnsureComp<UnremoveableComponent>(held.Value);
        }
    }

    private void OnUnstrapped(Entity<BlockHandsOnBuckleComponent> nest, ref UnstrappedEvent args)
    {
        _virtualItem.DeleteInHandsMatching(args.Buckle.Owner, nest.Owner);
    }

    private void OnCanAttack(EntityUid uid, BuckleComponent buckle, ref AttackAttemptEvent args)
    {
        if (buckle.BuckledTo is { } to && HasComp<BlockHandsOnBuckleComponent>(to))
            args.Cancel();
    }
}
