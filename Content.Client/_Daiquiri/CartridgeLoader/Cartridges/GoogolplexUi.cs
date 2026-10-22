using Content.Client.UserInterface.Fragments;
using Robust.Client.UserInterface;

namespace Content.Client._Daiquiri.CartridgeLoader.Cartridges;

public sealed partial class GoogolplexUi : UIFragment
{
    private GoogolplexUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new GoogolplexUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
    }
}
