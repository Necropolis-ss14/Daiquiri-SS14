namespace Content.Server._Daiquiri.Campfire;

/// <summary>
/// A campfire that heats items placed on it and keeps nearby mobs at a comfortable temperature.
/// </summary>
[RegisterComponent]
public sealed partial class CampfireComponent : Component
{
    /// <summary>
    /// Radius in tiles around the campfire that is kept at <see cref="TargetTemperature"/>.
    /// </summary>
    [DataField]
    public float Radius = 3f;

    /// <summary>
    /// Temperature (in kelvin) the area around the campfire is kept at. 293.15 K = 20 °C.
    /// </summary>
    [DataField]
    public float TargetTemperature = 293.15f;

    /// <summary>
    /// How many kelvin per second nearby mobs below the target temperature are warmed by.
    /// </summary>
    [DataField]
    public float WarmRate = 8f;

    /// <summary>
    /// Thermal power applied to items placed on the campfire, in watts.
    /// </summary>
    [DataField]
    public float HeatPower = 1200f;
}