using System.Linq;
using Content.Shared.Humanoid;
using Content.Shared._Daiquiri.Genetics.Hulk;
using Robust.Client.GameObjects;

namespace Content.Client._Daiquiri.Genetics.Hulk;

public sealed partial class HulkSystem : SharedHulkSystem
{
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HulkComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnShutdown(Entity<HulkComponent> ent, ref ComponentShutdown args)
    {
        var (uid, comp) = ent;

        if (TerminatingOrDeleted(uid))
            return;

        if (HasComp<HumanoidAppearanceComponent>(uid))
            return;

        if (!TryComp(uid, out SpriteComponent? sprite))
            return;

        var layerCount = sprite.AllLayers.Count();

        if (comp.NonHumanoidOldLayerData.Count != layerCount)
            return;

        var spriteEnt = (uid, sprite);
        for (var i = 0; i < layerCount; i++)
        {
            _sprite.LayerSetColor(spriteEnt, i, comp.NonHumanoidOldLayerData[i]);
        }
    }

    protected override void UpdateColorStartup(Entity<HulkComponent> hulk)
    {
        base.UpdateColorStartup(hulk);

        var (uid, comp) = hulk;

        if (HasComp<HumanoidAppearanceComponent>(uid))
            return;

        if (!TryComp(uid, out SpriteComponent? sprite))
            return;

        var spriteEnt = (uid, sprite);
        var layerCount = sprite.AllLayers.Count();
        for (var i = 0; i < layerCount; i++)
        {
            if (!_sprite.TryGetLayer(spriteEnt, i, out var layer, false))
                continue;

            comp.NonHumanoidOldLayerData.Add(layer.Color);
            _sprite.LayerSetColor(spriteEnt, i, comp.SkinColor);
        }
    }
}