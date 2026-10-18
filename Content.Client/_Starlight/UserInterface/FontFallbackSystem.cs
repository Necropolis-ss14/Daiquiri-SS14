using Content.Client.Stylesheets.Fonts;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.UserInterface;

public sealed partial class FontFallbackSystem : EntitySystem
{
    [Dependency] private IResourceCache _resourceCache = default!;
    [Dependency] private IPrototypeManager _prototypes = default!;
    [Dependency] private FontTagHijackHolder _fontHijack = default!;

    private NotoFontFamilyStack? _fallback;

    public override void Initialize()
    {
        base.Initialize();

        _fontHijack.Hijack = ResolveFont;
    }

    private Font? ResolveFont(ProtoId<FontPrototype> id, int size)
    {
        if (!_prototypes.TryIndex(id, out var prototype))
            return null;

        if (prototype.Path.ToString().Contains("/NotoSans", StringComparison.OrdinalIgnoreCase))
            return null;

        _fallback ??= new NotoFontFamilyStack(_resourceCache);

        var primary = new VectorFont(_resourceCache.GetResource<FontResource>(prototype.Path), size);
        return new StackedFont(primary, _fallback.GetFont(size));
    }
}
