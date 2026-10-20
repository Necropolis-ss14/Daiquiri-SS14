using System;
using Content.Shared._Daiquiri.Lobby;
using Robust.Shared.Network;

namespace Content.Client._Daiquiri.Lobby;

public interface IClientVinylGrants
{
    event Action? Updated;

    void Initialize();
    bool IsGranted(string vinylId);
}

/// <summary>
/// Daiquiri
/// Выданные игроку пластинки, приходят с сервера.
/// </summary>
public sealed partial class ClientVinylGrants : IClientVinylGrants
{
    [Dependency] private IClientNetManager _netMgr = default!;

    private HashSet<string> _granted = [];

    public event Action? Updated;

    public void Initialize()
    {
        _netMgr.RegisterNetMessage<MsgVinylGrants>(OnGrants);
    }

    private void OnGrants(MsgVinylGrants message)
    {
        _granted = new HashSet<string>(message.Vinyls);
        Updated?.Invoke();
    }

    public bool IsGranted(string vinylId) => _granted.Contains(vinylId);
}
