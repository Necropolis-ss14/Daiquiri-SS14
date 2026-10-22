using Content.Shared._Starlight.Language;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Language;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LanguageImplantComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<LanguagePrototype> Language = "GalacticCommon";
}
