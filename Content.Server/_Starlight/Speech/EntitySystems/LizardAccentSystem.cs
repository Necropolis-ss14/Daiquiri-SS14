using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server._Starlight.Speech.EntitySystems;

public sealed partial class LizardAccentSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;

    [GeneratedRegex("[сСзЗшШчЧ]+")]
    private static partial Regex RegexCyrillicHiss();

    [GeneratedRegex("s+")]
    private static partial Regex RegexLowerS();

    [GeneratedRegex("S+")]
    private static partial Regex RegexUpperS();

    [GeneratedRegex(@"(\w)x")]
    private static partial Regex RegexInternalX();

    [GeneratedRegex(@"\bx([\-|r|R]|\b)")]
    private static partial Regex RegexLowerEndX();

    [GeneratedRegex(@"\bX([\-|r|R]|\b)")]
    private static partial Regex RegexUpperEndX();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
    {
        // hissss
        args.Message.Text = RegexLowerS().Replace(args.Message.Text, "sss");
        // hiSSS
        args.Message.Text = RegexUpperS().Replace(args.Message.Text, "SSS");
        // ekssit
        args.Message.Text = RegexInternalX().Replace(args.Message.Text, "$1kss");
        // ecks
        args.Message.Text = RegexLowerEndX().Replace(args.Message.Text, "ecks$1");
        // eckS
        args.Message.Text = RegexUpperEndX().Replace(args.Message.Text, "ECKS$1");

        args.Message.Text = RegexCyrillicHiss().Replace(args.Message.Text, CyrillicHiss);
    }

    private string CyrillicHiss(Match match)
    {
        var letter = char.ToLowerInvariant(match.Value[0]);
        var hiss = letter switch
        {
            'с' or 'з' => 'с',
            'ш' => 'ш',
            _ => 'щ',
        };

        if (char.IsUpper(match.Value[0]))
            hiss = char.ToUpperInvariant(hiss);

        return new string(hiss, _random.Next(2, 4));
    }
}
