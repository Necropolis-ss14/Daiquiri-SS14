using System.Linq;
using Content.Server._Starlight.GhostTheme;
using Content.Shared._Starlight.GhostTheme;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Fun)]
public sealed partial class GhostThemeCommand : LocalizedEntityCommands
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IGhostThemeGrantManager _grants = default!;
    [Dependency] private IPrototypeManager _protos = default!;

    public override string Command => "grantghosttheme";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, _player), "username"),
            2 => CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<GhostThemePrototype>(), "themeId"),
            _ => CompletionResult.Empty,
        };
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteLine("Usage: grantghosttheme <username> <themeId> - unlock a ghost theme for the player.");
            return;
        }

        if (!_player.TryGetSessionByUsername(args[0], out var session))
        {
            shell.WriteError($"No player found with username '{args[0]}'.");
            return;
        }

        if (!_protos.TryIndex<GhostThemePrototype>(args[1], out _))
        {
            shell.WriteError($"No ghost theme found with id '{args[1]}'.");
            return;
        }

        _grants.Grant(session.UserId, args[1]);
        shell.WriteLine($"Ghost theme '{args[1]}' unlocked for {args[0]}.");
    }
}
