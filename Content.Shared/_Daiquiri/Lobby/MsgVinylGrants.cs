using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Lobby;

/// <summary>
/// Daiquiri
/// Список пластинок, выданных игроку вручную (команда grantvinyl).
/// </summary>
public sealed class MsgVinylGrants : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public HashSet<string> Vinyls = [];

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var count = buffer.ReadVariableInt32();
        Vinyls = new HashSet<string>(count);
        for (var i = 0; i < count; i++)
            Vinyls.Add(buffer.ReadString());
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(Vinyls.Count);
        foreach (var id in Vinyls)
            buffer.Write(id);
    }

    public override NetDeliveryMethod DeliveryMethod => NetDeliveryMethod.ReliableOrdered;
}
