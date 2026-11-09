using Content.Shared.Atmos;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Planets;

/// <summary>
/// A planet that the planet console can send a shuttle to. Picking one has the server generate a
/// fresh, already running map out of the biome template below.
/// </summary>
// The kind is left to the engine, which names it after the class: "planetDestination". Spelling it
// out as well is what the Robust analyzers complain about.
[Prototype]
public sealed partial class PlanetDestinationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Name shown on the console and used as the name of the generated map.
    /// </summary>
    [DataField(required: true)]
    public LocId Name = string.Empty;

    /// <summary>
    /// Short pitch shown on the planet's card.
    /// </summary>
    [DataField]
    public LocId Description = string.Empty;

    /// <summary>
    /// Biome the map is generated from, shared with the salvage expeditions.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<BiomeTemplatePrototype> Biome = string.Empty;

    /// <summary>
    /// Id of the parallax used for the animated strip on top of the planet's card. Kept as a
    /// plain string because parallax prototypes live in the client assembly.
    /// </summary>
    [DataField]
    public string? Parallax;

    /// <summary>
    /// Ambient light of the planet. Defaults to the daylight the biome system uses.
    /// </summary>
    [DataField]
    public Color MapLight = Color.FromHex("#D8B059");

    /// <summary>
    /// Surface temperature in degrees Celsius. Only the readout on the card is in Celsius,
    /// the atmosphere itself is built in Kelvin.
    /// </summary>
    [DataField]
    public float Temperature = 20f;

    /// <summary>
    /// Moles of each gas the planet's air is made of.
    /// </summary>
    [DataField]
    public Dictionary<Gas, float> Atmosphere = new()
    {
        [Gas.Oxygen] = 21.824779f,
        [Gas.Nitrogen] = 82.10312f,
    };

    /// <summary>
    /// Colour the card and the disk readout are painted in.
    /// </summary>
    [DataField]
    public Color Accent = Color.FromHex("#4FC3E8");

    /// <summary>
    /// Weather the planet can get, one of them picked at random every time the sky turns over.
    /// Each one carries its own visuals, its own sound and its own idea of what counts as
    /// shelter. A planet with none of them simply never has any weather.
    /// </summary>
    [DataField]
    public List<EntProtoId> Weathers = new();

    /// <summary>
    /// Damage dealt every second to whoever is caught out in the open during the storm.
    /// </summary>
    [DataField]
    public ProtoId<DamageTypePrototype> WeatherDamageType = "Cold";

    /// <summary>
    /// How much of <see cref="WeatherDamageType"/> the storm deals per second.
    /// </summary>
    [DataField]
    public float WeatherDamagePerSecond = 1f;

    /// <summary>
    /// Line written to everyone on the planet when the storm starts.
    /// </summary>
    [DataField]
    public LocId? WeatherStartMessage;

    /// <summary>
    /// Line written to everyone on the planet when the storm ends.
    /// </summary>
    [DataField]
    public LocId? WeatherEndMessage;

    /// <summary>
    /// Colour both of those lines are written in.
    /// </summary>
    [DataField]
    public Color WeatherColor = Color.White;

    /// <summary>
    /// Bolt the planet drops out of the sky now and then. Left out, no planet gets any.
    /// </summary>
    [DataField]
    public EntProtoId? Lightning;

    /// <summary>
    /// Which of <see cref="Weathers"/> brings the bolts with it. Only that weather gets any, so a
    /// planet can keep its rain quiet and its storm violent.
    /// </summary>
    [DataField]
    public EntProtoId LightningDuring = string.Empty;

    /// <summary>
    /// Chance of a bolt every second that weather is running. Meant to be very small: a strike is
    /// a sighting, not a hazard.
    /// </summary>
    [DataField]
    public float LightningChancePerSecond = 0.01f;

    /// <summary>
    /// How hard the bolt hits whoever it comes down on.
    /// </summary>
    [DataField]
    public float LightningShockDamage = 15f;

    /// <summary>
    /// Ore veins seeded through the planet's rock, the same way salvage expeditions seed theirs:
    /// each marker turns some of the biome's rock formations into the matching ore-bearing rock.
    /// </summary>
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

    /// <summary>
    /// Creatures seeded through the biome's marker layers, which spawn them on the open ground as
    /// the chunks around the players are generated. Each list is picked from at random per chunk.
    /// </summary>
    [DataField]
    public List<ProtoId<BiomeMarkerLayerPrototype>> MobMarkers = new();

    /// <summary>
    /// Lower sorts first on the console.
    /// </summary>
    [DataField]
    public int Order;
}
