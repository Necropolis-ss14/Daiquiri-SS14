using Robust.Shared.Serialization;

namespace Content.Shared._Daiquiri.Harpy;

[Serializable, NetSerializable]
public enum HarpyVisualLayers : byte
{
    Singing,
}

[Serializable, NetSerializable]
public enum SingingVisualLayer : byte
{
    True,
    False,
}
