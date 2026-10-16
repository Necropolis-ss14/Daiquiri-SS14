using System;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared._Starlight.Playtime;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Server._Daiquiri.Playtime;

public sealed partial class ServerPlaytimeManager : IServerPlaytimeManager
{
    [Dependency] private PlayTimeTrackingManager _playTime = default!;

    public TimeSpan GetOverallPlaytime(ICommonSession user)
    {
        return _playTime.GetOverallPlaytime(user);
    }
}
