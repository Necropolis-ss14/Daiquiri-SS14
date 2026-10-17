using Content.Client._Starlight.AIHelp.UI;
using Content.Shared._Starlight.AIHelp;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client._Starlight.AIHelp;

[UsedImplicitly]
public sealed partial class AIHelpUIController : UIController, IOnSystemChanged<AIHelpSystem>
{
    private AIHelpSystem? _aiHelp;
    private AIHelpWindow? _window;

    public void OnSystemLoaded(AIHelpSystem system)
    {
        _aiHelp = system;
        _aiHelp.OnAIHelpResponse += OnResponseReceived;
    }

    public void OnSystemUnloaded(AIHelpSystem system)
    {
        system.OnAIHelpResponse -= OnResponseReceived;
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

    private void EnsureWindow()
    {
        if (_window is { Disposed: false })
            return;

        _window = new AIHelpWindow();
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
}
