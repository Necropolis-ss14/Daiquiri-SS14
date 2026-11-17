// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Destructible.Thresholds;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;

namespace Content.Shared._Lavaland.Compat;

[RegisterComponent, NetworkedComponent]
public sealed partial class CombatPowerComponent : Component
{
    [DataField("factor")]
    public double Threat = 1.0;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class CosmicTransmutableComponent : Component
{
    [DataField(required: true)]
    public EntProtoId TransmutesTo;

    [DataField(required: true)]
    public EntProtoId RequiredGlyphType;
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HomingProjectileComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid? Target;

    [DataField, AutoNetworkedField]
    public float? HomingSpeed = 720f;

    [DataField]
    public Angle Tolerance = Angle.FromDegrees(1);

    [DataField]
    public float HomingTime = 0.1f;

    [ViewVariables(VVAccess.ReadOnly)]
    public float HomingAccumulator;
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MultishotComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool MultishotAffected;

    [DataField]
    public float MissChance = 0.2f;

    [DataField]
    public float SpreadMultiplier = 1.5f;

    [DataField]
    public float SpreadAddition = 5f;

    [DataField]
    public float HandDamageAmount;

    [DataField]
    public string HandDamageType = "Blunt";

    [DataField]
    public float StaminaDamage;

    [DataField]
    public string ExamineMessage = "multishot-component-examine";
}

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class WeatherSchedulerComponent : Component
{
    [DataField(required: true)]
    public List<WeatherStage> Stages = new();

    [DataField]
    public int Stage;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextUpdate;
}

[Serializable, DataDefinition]
public partial struct WeatherStage
{
    [DataField(required: true)]
    public MinMax Duration = new(0, 0);

    [DataField]
    public EntProtoId? Weather;

    [DataField]
    public LocId? Message;
}

[RegisterComponent, ComponentProtoName("LavalandTrail"), NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TrailComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color Color = Color.White;

    [DataField]
    public float Scale = 1f;

    [DataField, AutoNetworkedField]
    public float Frequency = 0.2f;

    [DataField, AutoNetworkedField]
    public float Lifetime = 1f;

    [DataField, AutoNetworkedField]
    public float LerpTime = 0.05f;

    [DataField]
    public float AlphaLerpAmount = 0.3f;

    [DataField]
    public float ScaleLerpAmount;

    [DataField]
    public string? Shader;

    [DataField, AutoNetworkedField]
    public SpriteSpecifier? Sprite;
}
