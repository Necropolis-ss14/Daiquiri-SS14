using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.PaperPlease;

[RegisterComponent, NetworkedComponent]
public sealed partial class PassportGuideComponent : Component
{
    [DataField]
    public int Version = 1;
}

[Serializable, NetSerializable]
public sealed class PassportGuideUiState : BoundUserInterfaceState
{
    public string Year = string.Empty;
    public string Convention = string.Empty;
    public string States = string.Empty;
    public string Rules = string.Empty;
}

[Serializable, NetSerializable]
public enum PassportGuideUiKey : byte
{
    Key
}
