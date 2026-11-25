using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Xeno.QueenHiveEye.UI;

[Serializable, NetSerializable]
public sealed class XenoQueenHiveChosenBuiMsg : BoundUserInterfaceMessage
{
    public NetEntity Target;
}
