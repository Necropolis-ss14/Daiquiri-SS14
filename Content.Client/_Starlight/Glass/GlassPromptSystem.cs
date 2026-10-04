using Content.Shared._Starlight.Glass;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Client._Starlight.Glass;

/// <summary>
/// Runs the local glassprompt command when the server asks for it (glassshow command).
/// </summary>
public sealed partial class GlassPromptSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;
    [Dependency] private IConsoleHost _console = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>(OnShowPrompt);
    }

    private void OnShowPrompt(GlassPromptShowMessage message)
    {
        Logger.InfoS("glassprompt", "Received GlassPromptShowMessage, executing local glassprompt command.");
        _console.ExecuteCommand("glassprompt");
    }
}
