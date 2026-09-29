using Robust.Shared.Configuration;
using Content.Shared.CCVar.CVarAccess;
using Content.Shared.Administration;

namespace Content.Shared._Starlight.CCVar;

[CVarDefs]
public sealed partial class StarlightCCVars
{
    /// <summary>
    /// Basic CCVars
    /// </summary>
    public static readonly CVarDef<string> LobbyChangelogsList =
        CVarDef.Create("lobby_changelog.list", "ChangelogStarlight.yml,Changelog.yml", CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<string> ServerName =
        CVarDef.Create("lobby.server_name", "LightLine", CVar.SERVER | CVar.REPLICATED);

    [CVarControl(AdminFlags.Adminchat)]
    public static readonly CVarDef<string> OverrideGamemodeName =
        CVarDef.Create("lobby.gamemode_name_override", "", CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);
    [CVarControl(AdminFlags.Adminchat)]
    public static readonly CVarDef<string> OverrideGamemodeDescription =
        CVarDef.Create("lobby.gamemode_desc_override", "", CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

    /// <summary>
    /// Whether the no EORG popup is enabled.
    /// </summary>
    public static readonly CVarDef<bool> RoundEndNoEorgPopup =
        CVarDef.Create("game.round_end_eorg_popup_enabled", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// Skip the no EORG popup.
    /// </summary>
    public static readonly CVarDef<bool> SkipRoundEndNoEorgPopup =
        CVarDef.Create("game.skip_round_end_eorg_popup", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Translucent "liquid glass" interface theme. Turn off for fully opaque panels.
    /// </summary>
    public static readonly CVarDef<bool> UIGlassTheme =
        CVarDef.Create("ui.glass_theme", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether manual transparency strength adjustment is enabled.
    /// </summary>
    public static readonly CVarDef<bool> UIGlassTransparencyEnabled =
        CVarDef.Create("ui.glass_transparency_enabled", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Glass transparency strength, 0-100 percent.
    /// </summary>
    public static readonly CVarDef<int> UIGlassTransparency =
        CVarDef.Create("ui.glass_transparency", 20, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether the glass accent color tint is applied.
    /// </summary>
    public static readonly CVarDef<bool> UIGlassAccentEnabled =
        CVarDef.Create("ui.glass_accent_enabled", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Glass accent tint color (hex).
    /// </summary>
    public static readonly CVarDef<string> UIGlassAccent =
        CVarDef.Create("ui.glass_accent", "#7fb2ff", CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether the first-run liquid glass prompt was already shown.
    /// </summary>
    public static readonly CVarDef<bool> UIGlassThemeSeen =
        CVarDef.Create("ui.glass_theme_seen", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// How long to display the EORG popup for.
    /// </summary>
    public static readonly CVarDef<float> RoundEndNoEorgPopupTime =
        CVarDef.Create("game.round_end_eorg_popup_time", 5f, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<string> ConfigFile =
        CVarDef.Create("config.file", "config.yml", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Disables pathfinding system updates and path generation. Intended for integration tests.
    /// </summary>
    public static readonly CVarDef<bool> DisablePathfinding =
        CVarDef.Create("starlight.pathfinding.disable", false, CVar.SERVERONLY);

    public static readonly CVarDef<bool> DisableLoadMapRule =
        CVarDef.Create("starlight.loadmaprule.disable", false, CVar.SERVERONLY);

    /// <summary>
    /// Forces the Tester tab to appear in the ghost themes menu even without the Tester role
    /// </summary>
    public static readonly CVarDef<bool> ForceTestersTab =
        CVarDef.Create("ghosttheme.forcetesterstab", false, CVar.CLIENTONLY | CVar.ARCHIVE,
            "forces the Testers ghost theme tab to be visible even if you have no unlocks.");
}
