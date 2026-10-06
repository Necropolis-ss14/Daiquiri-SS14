using System;
using System.Linq;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client.Lobby.UI;

/// <summary>
/// Label with old-player-style marquee scrolling when the text overflows.
/// </summary>
public sealed partial class MarqueeLabel : Label
{
    private const int ScrollMs = 250;
    private const string Gap = "   •   ";

    private string _fullText = string.Empty;
    private int _offset;
    private bool _running;
    private Content.Client.Audio.ContentAudioSystem? _audio;

    private bool AnimPaused()
    {
        try
        {
            _audio ??= IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<Content.Client.Audio.ContentAudioSystem>();
        }
        catch
        {
            return false;
        }
        return _audio?.LobbyMusicPaused ?? false;
    }

    /// <summary>
    /// Full text. Scrolls automatically when it does not fit.
    /// </summary>
    public string FullText
    {
        get => _fullText;
        set
        {
            _fullText = value ?? string.Empty;
            _offset = 0;
            UpdateScroll();
        }
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        _running = true;
        Tick();
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();
        _running = false;
    }

    private void Tick()
    {
        if (!_running)
            return;
        if (!AnimPaused())
            UpdateScroll();
        Timer.Spawn(ScrollMs, Tick);
    }

    private void UpdateScroll()
    {
        if (string.IsNullOrEmpty(_fullText))
        {
            Text = _fullText;
            return;
        }

        // Daiquiri: tile the text to fill the whole width so there is never an empty gap.
        var unit = _fullText + Gap;
        var need = unit.Length;
        if (Size.X > 0)
            need = Math.Max(need, (int) (Size.X / 6.6f) + unit.Length);
        var tiled = string.Concat(Enumerable.Repeat(unit, (need + unit.Length - 1) / unit.Length));
        _offset %= unit.Length;
        if (_offset < 0)
            _offset += unit.Length;
        Text = tiled[_offset..] + tiled[.._offset];
        _offset--;
        if (_offset < 0)
            _offset += unit.Length;
    }
}
