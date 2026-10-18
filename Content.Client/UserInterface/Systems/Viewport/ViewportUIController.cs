using System;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Viewport;

public sealed partial class ViewportUIController : UIController
{
    [Dependency] private IEyeManager _eyeManager = default!;
    [Dependency] private IPlayerManager _playerMan = default!;
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IConfigurationManager _configurationManager = default!;
    public static readonly Vector2i ViewportSize = (EyeManager.PixelsPerMeter * 21, EyeManager.PixelsPerMeter * 15);
    public const int ViewportHeight = 15;
    private MainViewport? Viewport => UIManager.ActiveScreen?.GetWidget<MainViewport>();

    public override void Initialize()
    {
        _configurationManager.OnValueChanged(CCVars.ViewportMinimumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportMaximumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportVerticalFit, _ => UpdateViewportRatio());

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        ReloadViewport();
    }

    private void UpdateViewportRatio()
    {
        if (Viewport == null)
        {
            return;
        }

        var min = _configurationManager.GetCVar(CCVars.ViewportMinimumWidth);
        var max = _configurationManager.GetCVar(CCVars.ViewportMaximumWidth);
        var width = _configurationManager.GetCVar(CCVars.ViewportWidth);
        var verticalfit = _configurationManager.GetCVar(CCVars.ViewportVerticalFit) && _configurationManager.GetCVar(CCVars.ViewportStretch);

        if (verticalfit)
        {
            width = GetVerticalFitWidth(min, max);
        }
        else if (width < min || width > max)
        {
            width = CCVars.ViewportWidth.DefaultValue;
        }

        Viewport.Viewport.ViewportSize = (EyeManager.PixelsPerMeter * width, EyeManager.PixelsPerMeter * ViewportHeight);
        Viewport.UpdateCfg();
    }

    /// <summary>
    ///     Calculates the viewport width (in tiles) so that the rendered world matches the aspect
    ///     ratio of the game window when vertical fit is enabled. That way the world fills the whole
    ///     screen instead of being letterboxed with black bars (for example on 16:9 monitors).
    /// </summary>
    private int GetVerticalFitWidth(int min, int max)
    {
        var pixelSize = Viewport?.PixelSize ?? Vector2i.Zero;
        if (pixelSize.X <= 0 || pixelSize.Y <= 0)
        {
            return max;
        }

        // Cap the view so that ultra-wide monitors do not get an absurd field of view.
        var aspect = Math.Clamp((float) pixelSize.X / pixelSize.Y, min / (float) ViewportHeight, 21f / 9f);

        // Always round up so the world slightly overflows the window instead of leaving a gap.
        return Math.Max(max, (int) MathF.Ceiling(ViewportHeight * aspect));
    }

    public void ReloadViewport()
    {
        if (Viewport == null)
        {
            return;
        }

        UpdateViewportRatio();
        Viewport.Viewport.HorizontalExpand = true;
        Viewport.Viewport.VerticalExpand = true;

        // Keep the viewport aspect ratio in sync with the window when it gets resized.
        Viewport.OnResized -= OnViewportResized;
        Viewport.OnResized += OnViewportResized;

        _eyeManager.MainViewport = Viewport.Viewport;
    }

    private void OnViewportResized()
    {
        UpdateViewportRatio();
    }

    public override void FrameUpdate(FrameEventArgs e)
    {
        if (Viewport == null)
        {
            return;
        }

        base.FrameUpdate(e);

        Viewport.Viewport.Eye = _eyeManager.CurrentEye;

        // verify that the current eye is not "null". Fuck IEyeManager.

        var ent = _playerMan.LocalEntity;
        if (_eyeManager.CurrentEye.Position != default || ent == null)
            return;

        _entMan.TryGetComponent(ent, out EyeComponent? eye);

        if (eye?.Eye == _eyeManager.CurrentEye
            && _entMan.GetComponent<TransformComponent>(ent.Value).MapID == MapId.Nullspace)
        {
            // nothing to worry about, the player is just in null space... actually that is probably a problem?
            return;
        }

        // Currently, this shouldn't happen. This likely happened because the main eye was set to null. When this
        // does happen it can create hard to troubleshoot bugs, so lets print some helpful warnings:
        Log.Warning($"Main viewport's eye is in nullspace (main eye is null?). Attached entity: {_entMan.ToPrettyString(ent.Value)}. Entity has eye comp: {eye != null}");
    }
}
