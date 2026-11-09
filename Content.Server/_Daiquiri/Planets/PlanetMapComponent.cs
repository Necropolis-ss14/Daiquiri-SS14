using Content.Shared._Daiquiri.Planets;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// Marks a generated map as the planet with the given destination id. The flight console looks
/// for this before generating anything, so a second course charted to the same planet flies to
/// the one that is already there instead of making a copy of it.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetMapComponent : Component
{
    /// <summary>
    /// Destination this map was generated for.
    /// </summary>
    [DataField]
    public ProtoId<PlanetDestinationPrototype> Planet = string.Empty;

    /// <summary>
    /// How many shuttles have already been sent here. Used to spread the arrivals out so they do
    /// not land inside each other.
    /// </summary>
    [DataField]
    public int Arrivals = 1;
}