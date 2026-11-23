using Robust.Shared.Configuration;

namespace Content.Shared.NullLink.CCVar;

public sealed partial class NullLinkCCVars
{
    public static readonly CVarDef<string> DonationTier1 =
        CVarDef.Create("nulllink.donation.tier1", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier2 =
        CVarDef.Create("nulllink.donation.tier2", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier3 =
        CVarDef.Create("nulllink.donation.tier3", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier4 =
        CVarDef.Create("nulllink.donation.tier4", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier5 =
        CVarDef.Create("nulllink.donation.tier5", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier6 =
        CVarDef.Create("nulllink.donation.tier6", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier7 =
        CVarDef.Create("nulllink.donation.tier7", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier8 =
        CVarDef.Create("nulllink.donation.tier8", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier9 =
        CVarDef.Create("nulllink.donation.tier9", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> DonationTier10 =
        CVarDef.Create("nulllink.donation.tier10", "", CVar.SERVERONLY);
}
