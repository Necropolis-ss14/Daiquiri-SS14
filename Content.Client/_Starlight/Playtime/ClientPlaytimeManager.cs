using System;
using Content.Shared._Starlight.Playtime;
using Robust.Shared.Player;

namespace Content.Client._Starlight.Playtime;

/// <summary>
/// Client stub: requirements are evaluated server-side, the client only shows descriptions.
/// </summary>
public sealed class ClientPlaytimeManager : IServerPlaytimeManager
{
    public TimeSpan GetOverallPlaytime(ICommonSession user)
    {
        return TimeSpan.Zero;
    }
}
