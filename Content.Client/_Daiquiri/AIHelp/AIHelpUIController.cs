using Content.Client._Daiquiri.AIHelp.UI;
using Content.Shared._Daiquiri.AIHelp;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.Player;
using Robust.Shared.Console;

namespace Content.Client._Daiquiri.AIHelp;

[UsedImplicitly]
public sealed partial class AIHelpUIController : UIController, IOnSystemChanged<AIHelpSystem>
{
    [Dependency] private ISharedAdminManager _admin = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IConsoleHost _console = default!;

    private AIHelpSystem? _aiHelp;
    private AIHelpWindow? _window;
    private AIHelpAdminWindow? _adminWindow;

    public override void Initialize()
    {
        _console.RegisterCommand("aihelp_log",
            Loc.GetString("cmd-aihelp-log-desc"),
            Loc.GetString("cmd-aihelp-log-help"),
            AdminLogCommand);
    }

    public void OnSystemLoaded(AIHelpSystem system)
    {
        _aiHelp = system;
        _aiHelp.OnAIHelpResponse += OnResponseReceived;
        _aiHelp.OnAdminLogUpdated += OnAdminLogReceived;
    }

    public void OnSystemUnloaded(AIHelpSystem system)
    {
        system.OnAIHelpResponse -= OnResponseReceived;
        system.OnAdminLogUpdated -= OnAdminLogReceived;
        _aiHelp = null;
    }

    public void ToggleWindow()
    {
        EnsureWindow();

        if (_window!.IsOpen)
        {
            _window.Close();
            return;
        }

        _window.OpenCentered();
        UIManager.ClickSound();
    }

    public void ToggleAdminWindow()
    {
        if (_playerManager.LocalSession is not { } session || !_admin.HasAdminFlag(session, AdminFlags.Adminhelp))
            return;

        EnsureAdminWindow();

        if (_adminWindow!.IsOpen)
        {
            _adminWindow.Close();
            return;
        }

        _aiHelp?.RequestAdminLog();
        _adminWindow.OpenCentered();
        UIManager.ClickSound();
    }

    private void AdminLogCommand(IConsoleShell shell, string argStr, string[] args)
    {
        if (_playerManager.LocalSession is not { } session || !_admin.HasAdminFlag(session, AdminFlags.Adminhelp))
        {
            shell.WriteError(Loc.GetString("ai-help-admin-no-access"));
            return;
        }

        ToggleAdminWindow();
    }

    private void EnsureWindow()
    {
        if (_window is { Disposed: false })
            return;

        _window = new AIHelpWindow();
        _window.SetAdmin(_playerManager.LocalSession is { } session
                         && _admin.HasAdminFlag(session, AdminFlags.Adminhelp));
        _window.OnOpenAdminLog += ToggleAdminWindow;
        _window.OnMessageSend += text =>
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            _window.AddMessage(text, true);
            _aiHelp?.Send(text);
        };
        _window.OnClear += () =>
        {
            _window?.ClearMessages();
            _aiHelp?.Reset();
        };
    }

    private void EnsureAdminWindow()
    {
        if (_adminWindow is { Disposed: false })
            return;

        _adminWindow = new AIHelpAdminWindow();
        _adminWindow.OnRefresh += () => _aiHelp?.RequestAdminLog();
        _adminWindow.OnCancelRequest += entryId => _aiHelp?.CancelRequest(entryId);
        _adminWindow.OnClearContext += userId => _aiHelp?.ResetPlayerContext(userId);
        _adminWindow.OnSetMuted += (userId, muted) => _aiHelp?.SetMuted(userId, muted);
    }

    private void OnResponseReceived(SharedAIHelpSystem.AIHelpResponseMessage message)
    {
        EnsureWindow();

        if (message.Typing)
        {
            _window!.SetTyping(Loc.GetString("ai-help-typing"));
            return;
        }

        _window!.SetTyping(string.Empty);

        if (string.IsNullOrEmpty(message.Text))
            return;

        _window.AddMessage(message.Text, false, message.IsError);
    }

    private void OnAdminLogReceived(SharedAIHelpSystem.AIHelpAdminLogMessage message)
    {
        if (_adminWindow is not { Disposed: false })
            return;

        _adminWindow.UpdateLog(message.Entries, message.Muted);
    }
}
