using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets;

/// <summary>
/// Builds a translucent "liquid glass" variant of a stylesheet by cloning its
/// rules and lowering the alpha of opaque <see cref="StyleBoxFlat"/> panels.
/// Textured window backgrounds (which have no alpha of their own) are made
/// translucent via modulate-self. Already-translucent boxes are kept as is.
/// </summary>
public static class GlassTheme
{
    private const float GlassBorderAlpha = 0.9f;
    private const float GlassBorderLighten = 0.15f;
    private const float GlassWhiten = 0.12f;
    private const float GlassAccentMix = 0.35f;
    private const float GlassMinAlpha = 0.25f;

    public sealed record GlassParams(float Alpha, Color? Accent);

    /// <summary>
    /// Transparency slider (0-100) to panel alpha.
    /// </summary>
    public static float TransparencyToAlpha(int percent)
    {
        var t = Math.Clamp(percent, 0, 100) / 100f;
        return MathF.Max(GlassMinAlpha, 1f - 0.75f * t);
    }

    public static Color? ParseAccent(string hex)
    {
        try
        {
            return Color.FromHex(hex);
        }
        catch
        {
            return null;
        }
    }

    public static GlassParams ReadParams(IConfigurationManager cfg)
    {
        var alpha = TransparencyToAlpha(cfg.GetCVar(Content.Shared._Starlight.CCVar.StarlightCCVars.UIGlassTransparency));
        Color? accent = null;
        if (cfg.GetCVar(Content.Shared._Starlight.CCVar.StarlightCCVars.UIGlassAccentEnabled))
            accent = ParseAccent(cfg.GetCVar(Content.Shared._Starlight.CCVar.StarlightCCVars.UIGlassAccent));
        return new GlassParams(alpha, accent);
    }

    public static Stylesheet MakeGlass(Stylesheet source, GlassParams pars, out int glassifiedBoxes)
    {
        var count = 0;
        var rules = new List<StyleRule>();
        foreach (var rule in source.Rules)
            rules.Add(CloneRule(rule, pars, ref count));
        glassifiedBoxes = count;
        return new Stylesheet(rules.ToArray());
    }

    private static StyleRule CloneRule(StyleRule rule, GlassParams pars, ref int count)
    {
        var props = new List<StyleProperty>();
        foreach (var prop in rule.Properties)
            props.Add(CloneProperty(prop, pars, ref count));
        // Textured panels (window backgrounds, chat panels, ...) have no alpha of
        // their own, so make them translucent via modulate-self instead.
        // Existing modulate-self tints (e.g. PDA) get their alpha scaled, not skipped.
        var panelIdx = props.FindIndex(p => p.Name == PanelContainer.StylePropertyPanel
            && p.Value is StyleBoxTexture);
        if (panelIdx >= 0)
        {
            var modIdx = props.FindIndex(p => p.Name == Control.StylePropertyModulateSelf
                && p.Value is Color c && c.A >= 0.99f);
            if (modIdx >= 0)
            {
                var tint = (Color) props[modIdx].Value;
                props[modIdx] = new StyleProperty(Control.StylePropertyModulateSelf,
                    new Color(tint.R, tint.G, tint.B, pars.Alpha));
                count++;
            }
            else if (!props.Any(p => p.Name == Control.StylePropertyModulateSelf))
            {
                props.Add(new StyleProperty(Control.StylePropertyModulateSelf,
                    new Color(1f, 1f, 1f, pars.Alpha)));
                count++;
            }
        }
        return new StyleRule(rule.Selector, props);
    }

    private static StyleProperty CloneProperty(StyleProperty property, GlassParams pars, ref int count)
    {
        if (property.Value is not StyleBoxFlat box)
            return property;

        var glass = new StyleBoxFlat(box);
        var bg = TintPanel(glass.BackgroundColor, pars);
        if (bg.A < glass.BackgroundColor.A)
            count++;
        glass.BackgroundColor = bg;
        glass.BorderColor = Glassify(glass.BorderColor, GlassBorderAlpha, GlassBorderLighten);
        return new StyleProperty(property.Name, glass);
    }

    private static Color TintPanel(Color color, GlassParams pars)
    {
        if (color.A < 0.99f)
            return color;

        Color target;
        float mix;
        if (pars.Accent is { } accent)
        {
            target = accent;
            mix = GlassAccentMix;
        }
        else
        {
            target = new Color(1f, 1f, 1f);
            mix = GlassWhiten;
        }
        var r = color.R + (target.R - color.R) * mix;
        var g = color.G + (target.G - color.G) * mix;
        var b = color.B + (target.B - color.B) * mix;
        return new Color(r, g, b, pars.Alpha);
    }

    /// <summary>
    /// Translucent tinted variant of an opaque panel color, for hardcoded backgrounds.
    /// </summary>
    public static Color GlassifyPanel(Color color, GlassParams pars)
    {
        return TintPanel(color, pars);
    }

    /// <summary>
    /// Legacy whiten-only variant.
    /// </summary>
    public static Color GlassifyPanel(Color color)
    {
        return TintPanel(color, new GlassParams(0.8f, null));
    }

    private static Color Glassify(Color color, float alpha, float lighten)
    {
        if (color.A < 0.99f)
            return color;

        var r = color.R + (1f - color.R) * lighten;
        var g = color.G + (1f - color.G) * lighten;
        var b = color.B + (1f - color.B) * lighten;
        return new Color(r, g, b, alpha);
    }
}
