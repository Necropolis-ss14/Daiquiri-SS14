using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Xeno.QueenHiveEye.UI;

[Serializable, NetSerializable]
public sealed class XenoQueenHiveBuiState : BoundUserInterfaceState
{
    public List<XenoCameraEntry> Entries = [];
}

[Serializable, NetSerializable]
public record struct XenoCameraEntry(NetEntity Entity, string Name);
