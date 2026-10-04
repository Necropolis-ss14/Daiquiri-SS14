using System.Linq;
using Content.Shared._Starlight.Glass;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed partial class GlassShowCommand : LocalizedEntityCommands
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private INetManager _net = default!;

    public override string Command => "glassshow";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = CompletionHelper.SessionNames(true, _player).ToList();
            options.Insert(0, new CompletionOption("all"));
            return CompletionResult.FromHintOptions(options, "username or all");
        }

        return CompletionResult.Empty;
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteLine("Usage: glassshow <username|all> - open the liquid glass prompt on player screen(s).");
            return;
        }

        if (args[0] == "all")
        {
            var count = 0;
            foreach (var session in _player.Sessions)
            {
                _net.ServerSendMessage(new GlassPromptShowMessage(), session.Channel);
                count++;
            }
            shell.WriteLine($"Glass prompt shown to {count} player(s).");
            return;
        }

        if (!_player.TryGetSessionByUsername(args[0], out var targetSession))
        {
            shell.WriteError($"No player found with username '{args[0]}'.");
            return;
        }

        _net.ServerSendMessage(new GlassPromptShowMessage(), targetSession.Channel);
        shell.WriteLine($"Glass prompt shown to {args[0]}.");
    }
}
