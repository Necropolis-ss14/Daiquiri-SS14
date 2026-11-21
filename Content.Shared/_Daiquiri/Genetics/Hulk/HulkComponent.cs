using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Daiquiri.Genetics.Hulk;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[AutoGenerateComponentPause]
public sealed partial class HulkComponent : Component
{
    [DataField]
    public float? Duration;

    [DataField, AutoNetworkedField]
    public float OldScale = 1f;

    [DataField]
    public Color? OldSkinColor;

    [DataField]
    public Color? OldEyeColor;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextRoar = TimeSpan.Zero;

    [DataField]
    public List<Color> NonHumanoidOldLayerData = new();

    [DataField]
    public float FistDamageMultiplier = 7f;

    [DataField]
    public float MaxBonusFistDamage = 50f;

    [DataField]
    public DamageSpecifier? StructuralDamage;

    [DataField]
    public Color SkinColor = Color.FromHex("#4EDB53");

    [DataField]
    public Color EyeColor = Color.FromHex("#910C17");

    [DataField]
    public TimeSpan RoarDelay = TimeSpan.FromSeconds(0.5);

    [DataField]
    public List<LocId> Roars = new()
    {
        "hulk-roar-1",
        "hulk-roar-2",
        "hulk-roar-3",
        "hulk-roar-4",
        "hulk-roar-5",
    };
}