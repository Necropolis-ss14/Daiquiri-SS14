using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Planets;

/// <summary>
/// A console that charts a course to a planet and, in doing so, has the server generate that
/// planet's map and jump the grid the console is on over to it.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PlanetConsoleComponent : Component
{
    /// <summary>
    /// Planets this console offers. When left empty every planet prototype is offered, sorted
    /// by its <see cref="PlanetDestinationPrototype.Order"/>.
    /// </summary>
    [DataField]
    public List<ProtoId<PlanetDestinationPrototype>> Planets = new();

    /// <summary>
    /// How long the console spends charting the course before the grid jumps.
    /// </summary>
    [DataField]
    public TimeSpan PrintTime = TimeSpan.FromSeconds(15);

    /// <summary>
    /// How long the console waits between two jumps. A single console charts one course per
    /// cooldown, so a crew cannot flood the server with planets.
    /// </summary>
    [DataField]
    public TimeSpan Cooldown = TimeSpan.FromMinutes(20);

    /// <summary>
    /// Sound played once when the console starts charting. It is the sound a generator makes when
    /// it is started, which fits a console that is about to work just as hard.
    /// </summary>
    [DataField]
    public SoundSpecifier PrintSound = new SoundCollectionSpecifier("GeneratorTug");

    /// <summary>
    /// Alarm the console sounds on a loop for as long as it is printing.
    /// </summary>
    [DataField]
    public SoundSpecifier PrintLoopSound = new SoundPathSpecifier("/Audio/Machines/alarm.ogg");

    /// <summary>
    /// Audio stream of the course being charted, so it can be cut off when the grid jumps.
    /// </summary>
    [ViewVariables]
    public EntityUid? PrintStream;

    /// <summary>
    /// Planet the console is currently charting a course to, if any.
    /// </summary>
    [ViewVariables]
    public string? Printing;

    /// <summary>
    /// When the course is finished and the grid jumps.
    /// </summary>
    [ViewVariables]
    public TimeSpan PrintEnd;
}
