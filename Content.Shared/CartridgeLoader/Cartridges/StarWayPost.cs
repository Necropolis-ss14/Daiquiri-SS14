using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed record StarWayPost(int Number, string Author, string Text, string Timestamp);
