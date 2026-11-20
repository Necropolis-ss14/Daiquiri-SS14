using System.Numerics;
using System.Text;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._Daiquiri.Planets;

public sealed class PlanetDiscoveryOverlay : Overlay
{
    public override OverlaySpace Space => OverlaySpace.ScreenSpace;

    private const float FadeInTime = 0.9f;
    private const float HoldTime = 3.4f;
    private const float FadeOutTime = 1.8f;
    private const float TotalTime = FadeInTime + HoldTime + FadeOutTime;

    private const float VerticalPosition = 0.17f;
    private const float FontScale = 1f;
    private const float MaxLetterSpacing = 12f;

    private const float RuleGap = 46f;
    private const float RuleThickness = 2f;

    private static readonly Color TextColor = Color.FromHex("#F6EFDD");
    private static readonly Color ShadowColor = Color.FromHex("#0B0B0D");

    private readonly IGameTiming _timing;
    private readonly Font _font;
    private readonly IUserInterfaceManager _ui;
    private readonly List<Vector2> _points = new(6);

    private string _name = string.Empty;
    private Color _accent = Color.White;
    private TimeSpan _shownAt;
    private bool _active;

    public PlanetDiscoveryOverlay(IGameTiming timing, Font font)
    {
        _timing = timing;
        _font = font;
        _ui = IoCManager.Resolve<IUserInterfaceManager>();
        ZIndex = 200;
    }

    public void Show(string name, Color accent)
    {
        _name = name;
        _accent = accent;
        _shownAt = _timing.RealTime;
        _active = true;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (!_active)
            return;

        var time = (float) (_timing.RealTime - _shownAt).TotalSeconds;

        if (time >= TotalTime)
        {
            _active = false;
            return;
        }

        var reveal = Smooth(Math.Clamp(time / FadeInTime, 0f, 1f));

        var alpha = time < FadeInTime
            ? reveal
            : time > FadeInTime + HoldTime
                ? 1f - Smooth((time - FadeInTime - HoldTime) / FadeOutTime)
                : 1f;

        if (alpha <= 0.001f)
            return;

        var handle = args.ScreenHandle;
        var bounds = args.ViewportBounds;
        var width = bounds.Right - bounds.Left;
        var height = bounds.Bottom - bounds.Top;
        var uiScale = _ui.RootControl.UIScale;

        var name = Loc.GetString(_name);
        var scale = FontScale * uiScale;
        var spacing = MaxLetterSpacing * uiScale * (0.35f + 0.65f * reveal);
        var textWidth = Measure(name, scale, spacing);
        var lineHeight = _font.GetLineHeight(scale);

        var centerX = bounds.Left + width / 2f;
        var centerY = bounds.Top + height * VerticalPosition + (1f - reveal) * 14f * uiScale;

        var frameAlpha = alpha * Smooth(Math.Clamp((time - 0.1f) / FadeInTime, 0f, 1f));
        var halfWidth = MathF.Max(textWidth / 2f + 56f * uiScale, 210f * uiScale) * (0.82f + 0.18f * reveal);
        var innerHalf = textWidth / 2f + 18f * uiScale;

        DrawFrame(handle, centerX, centerY, halfWidth, innerHalf, frameAlpha, uiScale);

        var textPos = new Vector2(centerX - textWidth / 2f, centerY - lineHeight / 2f);
        var shadowPos = textPos + new Vector2(0f, 4f * uiScale);

        DrawSpaced(handle, textPos, name, scale, spacing, Color.Transparent,
            new TextOutline(6f * uiScale, _accent.WithAlpha(alpha * 0.3f)));
        DrawSpaced(handle, shadowPos, name, scale, spacing, ShadowColor.WithAlpha(alpha * 0.55f), null);
        DrawSpaced(handle, textPos, name, scale, spacing, TextColor.WithAlpha(alpha),
            new TextOutline(3f * uiScale, ShadowColor.WithAlpha(alpha * 0.95f)));
    }

    private void DrawFrame(DrawingHandleScreen handle, float centerX, float centerY, float halfWidth, float innerHalf, float alpha, float uiScale)
    {
        if (alpha <= 0.001f)
            return;

        var line = _accent.WithAlpha(alpha * 0.8f);
        var gem = _accent.WithAlpha(alpha);
        var gemEdge = ShadowColor.WithAlpha(alpha * 0.75f);

        var gap = RuleGap * uiScale;
        var topY = centerY - gap;
        var bottomY = centerY + gap;

        DrawHLine(handle, centerX - halfWidth, centerX + halfWidth, topY, line, uiScale);
        DrawHLine(handle, centerX - halfWidth, centerX + halfWidth, bottomY, line, uiScale);

        DrawDiamond(handle, new Vector2(centerX, topY), 8.5f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX - halfWidth, topY), 6.5f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX + halfWidth, topY), 6.5f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX - halfWidth, bottomY), 5.5f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX + halfWidth, bottomY), 5.5f * uiScale, gem, gemEdge);

        DrawDiamond(handle, new Vector2(centerX - innerHalf, topY), 3.2f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX + innerHalf, topY), 3.2f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX - innerHalf, bottomY), 3.2f * uiScale, gem, gemEdge);
        DrawDiamond(handle, new Vector2(centerX + innerHalf, bottomY), 3.2f * uiScale, gem, gemEdge);
    }

    private void DrawSpaced(DrawingHandleScreen handle, Vector2 pos, string text, float scale, float spacing, Color fill, TextOutline? outline)
    {
        var x = pos.X;

        foreach (var rune in text.EnumerateRunes())
        {
            handle.DrawString(_font, new Vector2(x, pos.Y), rune.ToString(), scale, fill, outline);
            x += Advance(rune, scale) + spacing;
        }
    }

    private float Measure(string text, float scale, float spacing)
    {
        var width = 0f;
        var count = 0;

        foreach (var rune in text.EnumerateRunes())
        {
            width += Advance(rune, scale) + spacing;
            count++;
        }

        return count > 0 ? width - spacing : 0f;
    }

    private float Advance(Rune rune, float scale)
        => _font.GetCharMetrics(rune, scale)?.Advance ?? 0f;

    private void DrawHLine(DrawingHandleScreen handle, float x0, float x1, float y, Color color, float uiScale)
    {
        var half = RuleThickness * uiScale / 2f;

        _points.Clear();
        _points.Add(new Vector2(x0, y - half));
        _points.Add(new Vector2(x1, y - half));
        _points.Add(new Vector2(x1, y + half));
        _points.Add(new Vector2(x0, y - half));
        _points.Add(new Vector2(x1, y + half));
        _points.Add(new Vector2(x0, y + half));

        handle.DrawPrimitives(DrawPrimitiveTopology.TriangleList, _points, color);
    }

    private void DrawDiamond(DrawingHandleScreen handle, Vector2 center, float radius, Color fill, Color edge)
    {
        _points.Clear();
        _points.Add(center + new Vector2(0f, -radius));
        _points.Add(center + new Vector2(radius, 0f));
        _points.Add(center + new Vector2(0f, radius));
        _points.Add(center + new Vector2(-radius, 0f));

        handle.DrawPrimitives(DrawPrimitiveTopology.TriangleFan, _points, fill);
        handle.DrawLine(_points[0], _points[1], edge);
        handle.DrawLine(_points[1], _points[2], edge);
        handle.DrawLine(_points[2], _points[3], edge);
        handle.DrawLine(_points[3], _points[0], edge);
    }

    private static float Smooth(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return value * value * (3f - 2f * value);
    }
}
