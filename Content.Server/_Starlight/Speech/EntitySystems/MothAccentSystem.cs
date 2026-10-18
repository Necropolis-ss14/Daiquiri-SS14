using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server._Starlight.Speech.EntitySystems;

public sealed partial class MothAccentSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;

    [GeneratedRegex("z{1,3}", RegexOptions.IgnoreCase)]
    private static partial Regex RegexBuzz();

    [GeneratedRegex("[жЖзЗ]+")]
    private static partial Regex RegexCyrillicBuzz();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MothAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, MothAccentComponent component, AccentGetEvent args)
    {
        // buzzz - extend z sounds
        args.Message.Text = RegexBuzz().Replace(args.Message.Text, m =>
            char.IsUpper(m.Value[0]) ? "ZZZ" : "zzz");

        args.Message.Text = RegexCyrillicBuzz().Replace(args.Message.Text,
            m => new string(m.Value[0], _random.Next(2, 4)));
    }
}
