using Content.Server.Administration; // Daiquiri
using Content.Server._Daiquiri.Lobby;
using Content.Shared._Daiquiri.Lobby;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.Administration.Commands;

[AdminCommand(AdminFlags.VarEdit)]
public sealed partial class GrantVinylCommand : LocalizedCommands
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IEntitySystemManager _systems = default!;
    [Dependency] private IPrototypeManager _protos = default!;

    public override string Command => "grantvinyl";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, _player), "username"),
            2 => CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<LobbyVinylPrototype>(), "vinylId"),
            _ => CompletionResult.Empty,
        };
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteLine("Usage: grantvinyl <username> <vinylId> - give a record to the player.");
            return;
        }

        if (!_player.TryGetSessionByUsername(args[0], out var session))
        {
            shell.WriteError($"No player found with username '{args[0]}'.");
            return;
        }

        if (!_protos.TryIndex<LobbyVinylPrototype>(args[1], out _))
        {
            shell.WriteError($"No record found with id '{args[1]}'.");
            return;
        }

        _systems.GetEntitySystem<VinylGrantSystem>().Grant(session, args[1]);
        shell.WriteLine($"Record '{args[1]}' granted to {args[0]}.");
    }
}
