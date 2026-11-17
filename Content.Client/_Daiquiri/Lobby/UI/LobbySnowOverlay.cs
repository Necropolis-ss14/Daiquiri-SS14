using System.Numerics;
using Content.Shared._Daiquiri.CCVar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client._Daiquiri.Lobby.UI;

public sealed partial class LobbySnowOverlay : Control
{
    private sealed class Flake
    {
        public Vector2 Position;
        public float Speed;
        public float Drift;
        public float Phase;
        public float Alpha;
        public float Scale;
    }

    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private readonly List<Flake> _flakes = new();
    private Vector2 _cacheSize;
    private float _time;
    private bool _enabled = true;

    private const int FlakeCount = 220;
    private const float MinSpeed = 14f;
    private const float MaxSpeed = 46f;

    public LobbySnowOverlay()
    {
        IoCManager.InjectDependencies(this);
        MouseFilter = MouseFilterMode.Ignore;
        LayoutContainer.SetAnchorPreset(this, LayoutContainer.LayoutPreset.Wide);

        _enabled = _cfg.GetCVar(DaiquiriCCVars.LobbySnowEnabled);
        _cfg.OnValueChanged(DaiquiriCCVars.LobbySnowEnabled, OnSnowToggled);
        ApplyEnabled();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_enabled)
            return;

        var size = Size;
        if (size != _cacheSize)
        {
            _cacheSize = size;
            Rebuild(size);
        }

        if (size.X <= 1f || size.Y <= 1f || _flakes.Count == 0)
            return;

        _time += args.DeltaSeconds;

        foreach (var flake in _flakes)
        {
            flake.Position.Y += flake.Speed * args.DeltaSeconds;
            flake.Position.X += MathF.Sin(_time * flake.Drift + flake.Phase) * 10f * args.DeltaSeconds;

            if (flake.Position.Y > size.Y + 4f)
            {
                flake.Position = new Vector2(_random.NextFloat() * size.X, -_random.NextFloat() * 24f - 2f);
            }

            if (flake.Position.X < -6f)
                flake.Position.X += size.X + 12f;
            else if (flake.Position.X > size.X + 6f)
                flake.Position.X -= size.X + 12f;
        }
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        if (!_enabled || _flakes.Count == 0)
            return;

        foreach (var flake in _flakes)
        {
            var color = Color.White.WithAlpha(flake.Alpha);
            var p = flake.Position;
            var s = flake.Scale;

            handle.DrawRect(new UIBox2(p.X - 1.5f * s, p.Y - 0.5f * s, p.X + 1.5f * s, p.Y + 0.5f * s), color);
            handle.DrawRect(new UIBox2(p.X - 0.5f * s, p.Y - 1.5f * s, p.X + 0.5f * s, p.Y + 1.5f * s), color);
        }
    }

    private void OnSnowToggled(bool value)
    {
        _enabled = value;
        ApplyEnabled();
    }

    private void ApplyEnabled()
    {
        Visible = _enabled;

        if (!_enabled)
        {
            _flakes.Clear();
            _cacheSize = Vector2.Zero;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            _cfg.UnsubValueChanged(DaiquiriCCVars.LobbySnowEnabled, OnSnowToggled);
    }

    private void Rebuild(Vector2 size)
    {
        _flakes.Clear();

        if (size.X <= 1f || size.Y <= 1f)
            return;

        var count = Math.Clamp((int) (size.X * size.Y / 12000f), 40, FlakeCount);

        for (var i = 0; i < count; i++)
        {
            _flakes.Add(new Flake
            {
                Position = new Vector2(_random.NextFloat() * size.X, _random.NextFloat() * size.Y),
                Speed = MathHelper.Lerp(MinSpeed, MaxSpeed, _random.NextFloat()),
                Drift = MathHelper.Lerp(0.3f, 1.1f, _random.NextFloat()),
                Phase = _random.NextFloat() * MathF.Tau,
                Alpha = MathHelper.Lerp(0.55f, 1.0f, _random.NextFloat()),
                Scale = MathHelper.Lerp(0.6f, 1.7f, _random.NextFloat()),
            });
        }
    }
}