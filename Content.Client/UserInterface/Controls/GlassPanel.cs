using Content.Client.Stylesheets;
using Content.Shared._Starlight.CCVar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

/// <summary>
/// PanelContainer whose background follows the liquid glass theme toggle:
/// opaque <see cref="GlassColor"/> when glass is off, translucent whitened
/// version when it is on. Updates live on CVar change.
/// Use in place of hardcoded PanelOverride boxes in XAML.
/// </summary>
[Virtual]
public partial class GlassPanel : PanelContainer
{
    [Dependency] private IConfigurationManager _cfg = default!;

    private Color _glassColor = Color.Transparent;
    private Action<bool>? _onGlassBool;
    private Action<int>? _onGlassInt;
    private Action<string>? _onGlassString;

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
        _onGlassInt = _ => UpdateBox();
        _onGlassString = _ => UpdateBox();
        _cfg.OnValueChanged(StarlightCCVars.UIGlassTheme, _onGlassBool, true);
        _cfg.OnValueChanged(StarlightCCVars.UIGlassTransparencyEnabled, _onGlassBool);
        _cfg.OnValueChanged(StarlightCCVars.UIGlassTransparency, _onGlassInt);
        _cfg.OnValueChanged(StarlightCCVars.UIGlassAccentEnabled, _onGlassBool);
        _cfg.OnValueChanged(StarlightCCVars.UIGlassAccent, _onGlassString);
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();
        if (_onGlassBool != null)
        {
            _cfg.UnsubValueChanged(StarlightCCVars.UIGlassTheme, _onGlassBool);
            _cfg.UnsubValueChanged(StarlightCCVars.UIGlassTransparencyEnabled, _onGlassBool);
            _cfg.UnsubValueChanged(StarlightCCVars.UIGlassAccentEnabled, _onGlassBool);
        }
        if (_onGlassInt != null)
            _cfg.UnsubValueChanged(StarlightCCVars.UIGlassTransparency, _onGlassInt);
        if (_onGlassString != null)
            _cfg.UnsubValueChanged(StarlightCCVars.UIGlassAccent, _onGlassString);
    }

    private void UpdateBox()
    {
        if (_cfg == null!)
            return;

        var box = _cfg.GetCVar(StarlightCCVars.UIGlassTheme)
            ? new StyleBoxFlat(GlassTheme.GlassifyPanel(_glassColor, GlassTheme.ReadParams(_cfg)))
            : new StyleBoxFlat(_glassColor);
        if (_glassContentMargin >= 0f)
        {
            box.ContentMarginTopOverride = _glassContentMargin;
            box.ContentMarginBottomOverride = _glassContentMargin;
            box.ContentMarginLeftOverride = _glassContentMargin;
            box.ContentMarginRightOverride = _glassContentMargin;
        }
        PanelOverride = box;
    }
}
