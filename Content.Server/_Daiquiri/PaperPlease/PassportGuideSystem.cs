using Content.Shared._Daiquiri.PaperPlease;
using Content.Shared.UserInterface;

namespace Content.Server._Daiquiri.PaperPlease;

public sealed partial class PassportGuideSystem : EntitySystem
{
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PassportGuideComponent, BoundUIOpenedEvent>(OnOpened);
    }

    private void OnOpened(Entity<PassportGuideComponent> ent, ref BoundUIOpenedEvent args)
    {
        var version = Math.Clamp(ent.Comp.Version, 1, 3);
        _ui.SetUiState(ent.Owner, PassportGuideUiKey.Key, new PassportGuideUiState
        {
            Year = Loc.GetString("guide-tab-year"),
            Convention = Loc.GetString($"guide-{version}-convention"),
            States = Loc.GetString("guide-tab-states"),
            Rules = Loc.GetString($"guide-{version}-rules"),
        });
    }
}
