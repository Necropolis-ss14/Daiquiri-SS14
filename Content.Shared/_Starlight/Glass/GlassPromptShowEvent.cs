using System;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.Glass;

/// <summary>
/// Server asks a client to open the liquid glass first-run prompt (see glassshow command).
/// </summary>
[Serializable, NetSerializable]
public sealed class GlassPromptShowEvent : EntityEventArgs
{
}
