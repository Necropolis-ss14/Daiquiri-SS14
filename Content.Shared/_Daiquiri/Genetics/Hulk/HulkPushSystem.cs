using System.Numerics;
using Content.Shared.CombatMode;
using Content.Shared.Throwing;

namespace Content.Shared._Daiquiri.Genetics.Hulk;

public sealed partial class HulkPushSystem : EntitySystem
{
    [Dependency] private readonly ThrowingSystem _throwing = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DisarmedEvent>(OnDisarmed);
    }

    private void OnDisarmed(ref DisarmedEvent args)
    {
        if (!HasComp<HulkComponent>(args.Source))
            return;

        var sourcePos = Transform(args.Source).WorldPosition;
        var targetPos = Transform(args.Target).WorldPosition;
        var direction = targetPos - sourcePos;

        if (direction == Vector2.Zero)
            return;

        _throwing.TryThrow(args.Target, direction.Normalized() * 8f, 14f,
            user: args.Source, animated: false, playSound: false, doSpin: false);
    }
}
