using Content.Shared._Daiquiri.Xeno.QueenHiveEye.UI;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Daiquiri.Xeno.QueenHiveEye;

[UsedImplicitly]
public sealed class XenoQueenHiveBoundUserInterface : BoundUserInterface
{
    private XenoQueenHiveWindow? _window;

    public XenoQueenHiveBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = new XenoQueenHiveWindow();
        _window.OnClose += Close;
        Update(State);
        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState? state)
    {
        Update(state);
    }

    private void Update(BoundUserInterfaceState? state)
    {
        if (_window == null)
            return;

        _window.List.DisposeAllChildren();

        if (state is not XenoQueenHiveBuiState hiveState)
            return;

        foreach (var entry in hiveState.Entries)
        {
            var button = new Button { Text = entry.Name };
            var target = entry.Entity;
            button.OnPressed += _ => SendMessage(new XenoQueenHiveChosenBuiMsg { Target = target });
            _window.List.AddChild(button);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _window?.Dispose();
    }
}
