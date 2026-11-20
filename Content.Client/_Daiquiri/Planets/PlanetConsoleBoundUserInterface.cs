using System.Numerics;
using Content.Client._Daiquiri.Planets.UI;
using Content.Shared._Daiquiri.Planets;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._Daiquiri.Planets;

public sealed class PlanetConsoleBoundUserInterface : BoundUserInterface
{
    private PlanetConsoleWindow? _window;

    public PlanetConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindowCenteredLeft<PlanetConsoleWindow>();

        var root = IoCManager.Resolve<IUserInterfaceManager>().RootControl;
        var available = Vector2.Max(root.Size - new Vector2(24f, 24f), new Vector2(1f, 1f));
        if (_window.SetSize.X > available.X || _window.SetSize.Y > available.Y)
        {
            _window.SetSize = Vector2.Min(_window.SetSize, available);
            _window.Measure(Vector2Helpers.Infinity);
            _window.RecenterWindow(new Vector2(0.25f, 0.5f));
        }

        _window.OnPrint += planet => SendMessage(new PlanetConsolePrintMessage { Planet = planet });
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is PlanetConsoleState consoleState)
            _window?.UpdateState(consoleState);
    }
}