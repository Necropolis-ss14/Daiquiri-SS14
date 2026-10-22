using Content.Server.Speech.Components;
using Content.Shared.Humanoid;
using Robust.Shared.GameObjects;

namespace Content.Server._Daiquiri.Speech;

public sealed class FelinidAccentSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HumanoidAppearanceComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<HumanoidAppearanceComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.Species.Id == "Felinid")
            EnsureComp<OwOAccentComponent>(ent.Owner);
    }
}
