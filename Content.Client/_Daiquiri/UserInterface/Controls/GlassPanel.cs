using Content.Client._Daiquiri.Stylesheets;
using Content.Shared._Starlight.CCVar;
using Content.Shared._Starlight.UserInterface;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._Daiquiri.UserInterface.Controls;

/// <summary>
/// PanelContainer for hardcoded panel colours. While the Liquid Glass style theme is
/// active the background is drawn with the same frosted shader as the stylesheet panels;
/// otherwise it stays the plain opaque <see cref="GlassColor"/>. Updates live on CVar change.
/// Use in place of hardcoded PanelOverride boxes in XAML.
/// </summary>
[Virtual]
public partial class GlassPanel : PanelContainer
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IPrototypeManager _prototype = default!;

    private Color _glassColor = Color.Transparent;
    private Action<bool>? _onGlassBool;
    private Action<string>? _onGlassString;
    private Action<int>? _onGlassInt;
    private bool _hooked;

    /// <summary>
    /// Base (opaque) background color, settable from XAML.
    /// </summary>
    public Color GlassColor
    {
        get => _glassColor;
        set
        {
            _glassColor = value;
            UpdateBox();
        }
    }

    private float _glassContentMargin = -1f;

    /// <summary>
    /// Optional content margin applied to all sides (mirrors StyleBoxFlat ContentMargin*Override).
    /// </summary>
    public float GlassContentMargin
    {
        get => _glassContentMargin;
        set
        {
            _glassContentMargin = value;
            UpdateBox();
        }
    }

    public GlassPanel()
    {
        IoCManager.InjectDependencies(this);
        _onGlassBool = _ => UpdateBox();
        _onGlassString = _ => UpdateBox();
        _onGlassInt = _ => UpdateBox();
        Hook();
    }

    /// <summary>
    /// Panels are regularly taken out of the tree and put back (the character editor is
    /// detached when the lobby unloads or pops out, the store window closes and reopens).
    /// A panel that is out of the tree cannot see CVar changes, so re-hook and re-evaluate
    /// on the way back in - otherwise it keeps whatever background it had when it left.
    /// </summary>
    protected override void EnteredTree()
    {
        base.EnteredTree();
        Hook();
        // Hook() is a no-op when we are already subscribed, so re-read the theme here:
        // it may have changed while we were out of the tree.
        UpdateBox();
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();
        Unhook();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            Unhook();
    }

    private void Hook()
    {
        if (_hooked)
            return;

        _hooked = true;

        // Daiquiri
        _cfg.OnValueChanged(StarlightCCVars.StyleTheme, _onGlassString!, true);
        _cfg.OnValueChanged(StarlightCCVars.StyleAccent, _onGlassString!);
        _cfg.OnValueChanged(StarlightCCVars.StyleCustomAccent, _onGlassBool!);

        // The frosted box is built from these, so it has to follow the sliders in the
        // Liquid Glass settings window like the stylesheet panels do.
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassEnabled, _onGlassBool!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassBlur, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassRefraction, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassHighlight, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassOpacity, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassTint, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassTintEnabled, _onGlassBool!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassTintColor, _onGlassString!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassCornerRadius, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassQuality, _onGlassInt!);
        _cfg.OnValueChanged(StarlightCCVars.UILiquidGlassAnimate, _onGlassBool!);
    }

    private void Unhook()
    {
        if (!_hooked)
            return;

        _hooked = false;
        _cfg.UnsubValueChanged(StarlightCCVars.StyleTheme, _onGlassString!);
        _cfg.UnsubValueChanged(StarlightCCVars.StyleAccent, _onGlassString!);
        _cfg.UnsubValueChanged(StarlightCCVars.StyleCustomAccent, _onGlassBool!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassEnabled, _onGlassBool!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassBlur, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassRefraction, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassHighlight, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassOpacity, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassTint, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassTintEnabled, _onGlassBool!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassTintColor, _onGlassString!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassCornerRadius, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassQuality, _onGlassInt!);
        _cfg.UnsubValueChanged(StarlightCCVars.UILiquidGlassAnimate, _onGlassBool!);
    }

    private void UpdateBox()
    {
        if (_cfg == null!)
            return;

        // While the Liquid Glass theme is active, hardcoded opaque panels get the same
        // frosted backdrop as the stylesheet ones. Already translucent colours (barely
        // visible separators and the like) keep their look.
        var pars = GlassTheme.ReadLiquidParams(_cfg);
        var useGlass = pars.Enabled && _glassColor.A >= 0.99f && IsLiquidGlassTheme();

        var box = useGlass
            ? new LiquidGlassStyleBox(new StyleBoxFlat(_glassColor), pars)
            : (StyleBox) new StyleBoxFlat(_glassColor);
        if (_glassContentMargin >= 0f)
        {
            box.ContentMarginTopOverride = _glassContentMargin;
            box.ContentMarginBottomOverride = _glassContentMargin;
            box.ContentMarginLeftOverride = _glassContentMargin;
            box.ContentMarginRightOverride = _glassContentMargin;
        }
        PanelOverride = box;
    }

    private bool IsLiquidGlassTheme()
    {
        return _prototype.TryIndex<StyleThemePrototype>(
                   _cfg.GetCVar(StarlightCCVars.StyleTheme), out var proto)
               && proto.LiquidGlass;
    }
}
