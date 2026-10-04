using Content.Client.Options.UI;
using Content.Shared._Starlight.Glass;
using Robust.Client.UserInterface;
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
    [Dependency] private IUserInterfaceManager _ui = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>(OnShowPrompt);
    }

    private void OnShowPrompt(GlassPromptShowMessage message)
    {
        Logger.InfoS("glassprompt", "Received GlassPromptShowMessage, opening prompt window.");
        _ui.CreateWindow<GlassThemePrompt>().OpenCentered();
    }
}
