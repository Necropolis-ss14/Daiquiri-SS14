using System.Text;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Shared._Daiquiri.Genetics.Speech;

public sealed partial class CharactersAccentSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;

    private readonly StringBuilder _builder = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CharactersAccentComponent, AccentGetEvent>(OnAccentGet);
    }

    private void OnAccentGet(Entity<CharactersAccentComponent> ent, ref AccentGetEvent args)
    {
        _builder.Clear();
        var chars = ent.Comp.Chars;
        foreach (var c in args.Message.Text)
        {
            if (!chars.TryGetValue(c, out var replacements))
            {
                _builder.Append(c);
                continue;
            }

            _builder.Append(_random.Pick(replacements));
        }

        args.Message.Text = _builder.ToString();
        args.Message.Tts = args.Message.Text;
    }
}