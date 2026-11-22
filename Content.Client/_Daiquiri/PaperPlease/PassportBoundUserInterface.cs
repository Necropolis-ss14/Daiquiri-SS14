using Content.Shared._Daiquiri.PaperPlease;
using Robust.Client.UserInterface;

namespace Content.Client._Daiquiri.PaperPlease;

public sealed class PassportBoundUserInterface : BoundUserInterface
{
    private PassportWindow? _window;

    public PassportBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<PassportWindow>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is PassportUiState passport)
            _window?.Update(passport);
    }
}
