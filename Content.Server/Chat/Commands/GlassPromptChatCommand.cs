using Content.Shared._Starlight.Glass;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Chat.Commands;

/// <summary>
/// Lets any player open the liquid glass prompt on their own screen.
/// </summary>
[AnyCommand]
internal sealed partial class GlassPromptChatCommand : LocalizedEntityCommands
{
    [Dependency] private INetManager _net = default!;

    public override string Command => "glassprompt";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        if (player.Status != SessionStatus.InGame && player.Status != SessionStatus.Connected)
            return;

        _net.ServerSendMessage(new GlassPromptShowMessage(), player.Channel);
    }
}
