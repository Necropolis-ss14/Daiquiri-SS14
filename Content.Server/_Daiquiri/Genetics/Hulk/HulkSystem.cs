using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared.Chat;
using Content.Shared.Humanoid;
using Content.Shared.Sprite;
using Content.Shared._Daiquiri.Genetics.Hulk;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.Genetics.Hulk;

public sealed partial class HulkSystem : SharedHulkSystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private PhysicsSystem _physics = default!;
    [Dependency] private SharedHumanoidAppearanceSystem _humanoid = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private ChatSystem _chat = default!;
    [Dependency] private SharedScaleVisualsSystem _scale = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HulkComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<HulkComponent, ComponentRemove>(OnRemove);
    }

    private void OnInit(Entity<HulkComponent> ent, ref ComponentInit args)
    {
        var (uid, comp) = ent;

        Scale(uid, 1.25f);

        if (TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
        {
            comp.OldSkinColor = humanoid.SkinColor;
            comp.OldEyeColor = humanoid.EyeColor;
            _humanoid.SetSkinColor(uid, comp.SkinColor, humanoid: humanoid);
        }
        else
        {
            comp.OldSkinColor = null;
            comp.OldEyeColor = null;
        }
    }

    private void OnRemove(Entity<HulkComponent> ent, ref ComponentRemove args)
    {
        var (uid, comp) = ent;

        if (TerminatingOrDeleted(uid))
            return;

        Scale(uid, 0.8f);

        if (TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
        {
            if (comp.OldEyeColor is { } eyeColor)
                _humanoid.SetEyeColor(uid, eyeColor, humanoid: humanoid);
            if (comp.OldSkinColor is { } skinColor)
                _humanoid.SetSkinColor(uid, skinColor, humanoid: humanoid);
        }

        _popup.PopupEntity(Loc.GetString("hulk-unhulked"), uid, uid);
    }

    public override void Roar(Entity<HulkComponent> hulk, float prob = 1f)
    {
        base.Roar(hulk, prob);

        var (uid, comp) = hulk;

        if (comp.NextRoar >= _timing.CurTime)
            return;

        if (prob < 1f && !_random.Prob(prob))
            return;

        comp.NextRoar = _timing.CurTime + comp.RoarDelay;

        var speech = _random.Pick(comp.Roars);

        _chat.TrySendInGameICMessage(uid, Loc.GetString(speech), InGameICChatType.Speak, false);
    }

    private void Scale(EntityUid uid, float scale)
    {
        _scale.SetSpriteScale(uid, _scale.GetSpriteScale(uid) * scale);

        if (!TryComp(uid, out FixturesComponent? manager))
            return;

        foreach (var (id, fixture) in manager.Fixtures)
        {
            switch (fixture.Shape)
            {
                case PhysShapeCircle circle:
                    _physics.SetPositionRadius(uid,
                        id,
                        fixture,
                        circle,
                        circle.Position * scale,
                        circle.Radius * scale,
                        manager);
                    break;
                case PolygonShape poly:
                    var verts = poly.Vertices;

                    for (var i = 0; i < poly.VertexCount; i++)
                    {
                        verts[i] *= scale;
                    }

                    _physics.SetVertices(uid, id, fixture, poly, verts, manager);
                    break;
            }
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<HulkComponent>();
        while (query.MoveNext(out var ent, out var hulk))
        {
            if (hulk.Duration == null)
                continue;

            hulk.Duration -= frameTime;

            if (hulk.Duration >= 0)
                continue;

            RemCompDeferred<HulkComponent>(ent);
        }
    }
}