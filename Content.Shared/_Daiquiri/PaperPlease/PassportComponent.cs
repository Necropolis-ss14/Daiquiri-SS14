using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.PaperPlease;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PassportComponent : Component
{
    [DataField, AutoNetworkedField]
    public string State = string.Empty;

    [DataField, AutoNetworkedField]
    public string StateName = string.Empty;

    [DataField, AutoNetworkedField]
    public Color StateColor = Color.White;

    [DataField, AutoNetworkedField]
    public string IssueCity = string.Empty;

    [DataField, AutoNetworkedField]
    public int IssueYear;

    [DataField, AutoNetworkedField]
    public int ExpiryYear;

    [DataField, AutoNetworkedField]
    public string OwnerName = string.Empty;

    [DataField, AutoNetworkedField]
    public string Series = string.Empty;

    [DataField, AutoNetworkedField]
    public string Number = string.Empty;

    [DataField, AutoNetworkedField]
    public NetEntity? Owner;

    [DataField, AutoNetworkedField]
    public bool Generated;

    [DataField, AutoNetworkedField]
    public bool HasStamp = true;

    [DataField, AutoNetworkedField]
    public bool HasSignature = true;

    [DataField]
    public bool RandomState;

    [DataField]
    public string CorruptionReason = string.Empty;

    [DataField]
    public bool Bound;

    [DataField]
    public bool NameLocked;

    [DataField]
    public bool ForcedValid;

    [DataField]
    public bool ForcedInvalid;
}

[Serializable, NetSerializable]
public sealed class PassportUiState : BoundUserInterfaceState
{
    public string StateName;
    public Color StateColor;
    public string IssueCity;
    public int IssueYear;
    public int ExpiryYear;
    public string OwnerName;
    public string Series;
    public string Number;
    public bool HasStamp;
    public List<(string Name, Color Color)> Stamps = new();
    public bool HasSignature;
    public NetEntity? Owner;

    public PassportUiState(PassportComponent comp, NetEntity? owner)
    {
        StateName = comp.StateName;
        StateColor = comp.StateColor;
        IssueCity = comp.IssueCity;
        IssueYear = comp.IssueYear;
        ExpiryYear = comp.ExpiryYear;
        OwnerName = comp.OwnerName;
        Series = comp.Series;
        Number = comp.Number;
        HasStamp = comp.HasStamp;
        HasSignature = comp.HasSignature;
        Owner = owner;
    }
}

[Serializable, NetSerializable]
public enum PassportUiKey : byte
{
    Key
}
