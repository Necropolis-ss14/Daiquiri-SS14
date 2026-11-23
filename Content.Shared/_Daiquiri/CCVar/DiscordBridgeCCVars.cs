using Robust.Shared.Configuration;

namespace Content.Shared._Daiquiri.CCVar;

[CVarDefs]
public sealed class DiscordBridgeCCVars
{
    public static readonly CVarDef<bool> Enabled =
        CVarDef.Create("discord_bridge.enabled", false, CVar.SERVERONLY);

    public static readonly CVarDef<string> Url =
        CVarDef.Create("discord_bridge.url", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> Token =
        CVarDef.Create("discord_bridge.token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<int> RefreshSeconds =
        CVarDef.Create("discord_bridge.refresh_seconds", 300, CVar.SERVERONLY);
}
