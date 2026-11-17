using System.Linq;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.DropPod;

[AdminCommand(AdminFlags.Admin)]
public sealed partial class DropPodCommand : IConsoleCommand
{
    [Dependency] private IEntityManager _entManager = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    public string Command => "droppod";
    public string Description => "Drops a supply pod from the sky at your current position, delivering the chosen prototype.";
    public string Help => "droppod <entity prototype id> [amount]";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player?.AttachedEntity is not { } player)
        {
            shell.WriteError(Loc.GetString("droppod-player-only"));
            return;
        }

        if (args.Length is < 1 or > 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }

        if (!_proto.TryIndex<EntityPrototype>(args[0], out var prototype) || prototype.Abstract)
        {
            shell.WriteError(Loc.GetString("droppod-unknown-prototype", ("prototype", args[0])));
            return;
        }

        var amount = 1;
        if (args.Length == 2 && (!int.TryParse(args[1], out amount) || amount < 1))
        {
            shell.WriteError(Loc.GetString("droppod-invalid-amount"));
            return;
        }

        var coordinates = _entManager.GetComponent<TransformComponent>(player).Coordinates;
        _entManager.System<DropPodSystem>().Drop(coordinates, prototype.ID, amount);
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(
                _proto.EnumeratePrototypes<EntityPrototype>()
                    .Where(p => !p.Abstract)
                    .Select(p => p.ID)
                    .Order(),
                Loc.GetString("droppod-completion-prototype")),
            2 => CompletionResult.FromHint(Loc.GetString("droppod-completion-amount")),
            _ => CompletionResult.Empty,
        };
    }
}
