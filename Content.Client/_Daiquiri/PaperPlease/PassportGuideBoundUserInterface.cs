using Content.Shared._Daiquiri.PaperPlease;
using Robust.Client.UserInterface;

namespace Content.Client._Daiquiri.PaperPlease;

public sealed class PassportGuideBoundUserInterface : BoundUserInterface
{
    private PassportGuideWindow? _window;

    public PassportGuideBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<PassportGuideWindow>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is PassportGuideUiState guide)
            _window?.Update(guide);
    }
}
