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
            return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, _player), "username");

        return CompletionResult.Empty;
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteLine("Usage: glassshow <username> - open the liquid glass prompt on the player's screen.");
            return;
        }

        if (!_player.TryGetSessionByUsername(args[0], out var session))
        {
            shell.WriteError($"No player found with username '{args[0]}'.");
            return;
        }

        _net.ServerSendMessage(new GlassPromptShowMessage(), session.Channel);
        shell.WriteLine($"Glass prompt shown to {args[0]}.");
    }
}
