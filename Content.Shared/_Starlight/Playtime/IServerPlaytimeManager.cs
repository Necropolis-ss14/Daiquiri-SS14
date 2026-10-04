using System;
using Robust.Shared.Player;

namespace Content.Shared._Starlight.Playtime;

/// <summary>
/// Local server overall playtime, for requirements.
/// </summary>
public interface IServerPlaytimeManager
{
    TimeSpan GetOverallPlaytime(ICommonSession user);
}
