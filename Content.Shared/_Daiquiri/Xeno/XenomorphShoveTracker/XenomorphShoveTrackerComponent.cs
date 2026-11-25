using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Xeno.XenomorphShoveTracker;

[RegisterComponent, NetworkedComponent]
public sealed partial class XenomorphShoveTrackerComponent : Component
{
    [DataField]
    public Dictionary<EntityUid, int> ShoveCount = new();

    [DataField]
    public int ShoveThreshold = 3;

    [DataField]
    public TimeSpan KnockdownDuration = TimeSpan.FromSeconds(7);

    [DataField]
    public TimeSpan ShoveResetTime = TimeSpan.FromSeconds(5);

    [DataField]
    public Dictionary<EntityUid, TimeSpan> LastShoveTime = new();
}
