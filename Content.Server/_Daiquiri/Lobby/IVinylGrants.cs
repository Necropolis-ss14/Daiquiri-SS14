using System;
using System.Collections.Generic;

namespace Content.Server._Daiquiri.Lobby;

/// <summary>
/// Daiquiri
/// Выданные вручную пластинки.
/// </summary>
public interface IVinylGrants
{
    bool HasGrant(Guid userId, string vinylId);
    HashSet<string> GetGrants(Guid userId);
    void Grant(Guid userId, string vinylId);
}
