using Content.Client.Options.UI;
using Content.Shared._Starlight.Glass;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client._Starlight.Glass;

/// <summary>
/// Opens the liquid glass prompt when the server asks for it (glassshow command).
/// </summary>
public sealed class GlassPromptSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IUserInterfaceManager _ui = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<GlassPromptShowMessage>(OnShowPrompt);
    }

    private void OnShowPrompt(GlassPromptShowMessage message)
    {
        _ui.CreateWindow<GlassThemePrompt>().OpenCentered();
    }
}
