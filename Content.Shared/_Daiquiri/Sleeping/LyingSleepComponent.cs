using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Sleeping;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LyingSleepComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? SleepAction;
}
