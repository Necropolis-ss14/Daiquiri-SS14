using Content.Shared._Daiquiri.Language;
using Content.Shared._Starlight.Language;
using Content.Shared._Starlight.Language.Systems;
using Content.Shared.Implants;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.Language;

public sealed partial class LanguageImplantSystem : EntitySystem
{
    [Dependency] private SharedLanguageSystem _language = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LanguageImplantComponent, ImplantImplantedEvent>(OnImplantImplanted);
    }

    private void OnImplantImplanted(Entity<LanguageImplantComponent> ent, ref ImplantImplantedEvent args)
    {
        if (!_prototype.TryIndex(ent.Comp.Language, out LanguagePrototype? language))
            return;

        _language.AddLanguage(args.Implanted, ent.Comp.Language, true, true);
        _popup.PopupEntity(Loc.GetString("language-implant-success", ("language", language.Name)), args.Implanted, args.Implanted);
    }
}
