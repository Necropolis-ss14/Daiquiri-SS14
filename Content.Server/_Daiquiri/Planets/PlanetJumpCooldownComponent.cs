namespace Content.Server._Daiquiri.Planets;

/// <summary>
/// Recharge of a grid after a planet jump. It is kept on the grid rather than on the console so
/// that taking the console apart and building a new one does not hand out a free jump.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetJumpCooldownComponent : Component
{
    /// <summary>
    /// Earliest moment this grid may jump to a planet again. Serialised so the recharge survives
    /// the grid being saved and loaded; the component is server-only, so it is never dirtied for
    /// the network and the map save picks it up on its own.
    /// </summary>
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextJump;
}