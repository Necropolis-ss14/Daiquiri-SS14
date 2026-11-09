using Content.Shared.Atmos;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Planets;

[Prototype]
public sealed partial class PlanetDestinationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public LocId Name = string.Empty;

    [DataField]
    public LocId Description = string.Empty;

    [DataField(required: true)]
    public ProtoId<BiomeTemplatePrototype> Biome = string.Empty;

    [DataField]
    public string? Parallax;

    [DataField]
    public Color MapLight = Color.FromHex("#D8B059");

    [DataField]
    public float Temperature = 20f;

    [DataField]
    public Dictionary<Gas, float> Atmosphere = new()
    {
        [Gas.Oxygen] = 21.824779f,
        [Gas.Nitrogen] = 82.10312f,
    };

    [DataField]
    public Color Accent = Color.FromHex("#4FC3E8");

    [DataField]
    public List<EntProtoId> Weathers = new();

    [DataField]
    public ProtoId<DamageTypePrototype> WeatherDamageType = "Cold";

    [DataField]
    public float WeatherDamagePerSecond = 1f;

    [DataField]
    public LocId? WeatherStartMessage;

    [DataField]
    public LocId? WeatherEndMessage;

    [DataField]
    public Color WeatherColor = Color.White;

    [DataField]
    public EntProtoId? Lightning;

    [DataField]
    public EntProtoId LightningDuring = string.Empty;

    [DataField]
    public float LightningChancePerSecond = 0.01f;

    [DataField]
    public float LightningShockDamage = 15f;

    [DataField]
    public List<ProtoId<BiomeMarkerLayerPrototype>> OreMarkers = new()
    {
        "OreIron",
        "OreQuartz",
        "OreCoal",
        "OreSalt",
        "OreGold",
        "OreSilver",
        "OrePlasma",
        "OreUranium",
        "OreDiamond",
        "OreArtifactFragment",
    };

    [DataField]
    public List<ProtoId<BiomeMarkerLayerPrototype>> MobMarkers = new();

    [DataField]
    public int Order;
}
