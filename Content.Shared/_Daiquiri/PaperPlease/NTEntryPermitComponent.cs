using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.PaperPlease;

[RegisterComponent, NetworkedComponent]
public sealed partial class NTEntryPermitComponent : Component
{
    [DataField]
    public string LocId = "nt-entry-permit-valid";

    [DataField]
    public NetEntity? Owner;

    [DataField]
    public bool Bound;

    [DataField]
    public bool NameLocked;
}
