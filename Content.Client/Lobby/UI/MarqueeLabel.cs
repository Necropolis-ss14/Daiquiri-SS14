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
        if (!NeedsScroll())
        {
            if (Text != _fullText)
                Text = _fullText;
            return;
        }

        var loop = _fullText + Gap;
        if (loop.Length == 0)
        {
            Text = _fullText;
            return;
        }

        _offset %= loop.Length;
        if (_offset < 0)
            _offset += loop.Length;
        Text = loop[_offset..] + loop[.._offset];
        _offset--;
        if (_offset < 0)
            _offset += loop.Length;
    }

    private bool NeedsScroll()
    {
        if (string.IsNullOrEmpty(_fullText) || Size.X <= 0)
            return false;
        // Always scroll: classic player feel even for short titles.
        return true;
    }
}
