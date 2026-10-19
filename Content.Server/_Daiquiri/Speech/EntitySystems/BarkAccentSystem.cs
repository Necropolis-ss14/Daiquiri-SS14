using Content.Shared.StatusEffectNew;
using Content.Server.Speech.Components;
using Content.Shared.Speech;
using Robust.Shared.Random;
using Content.Shared._Starlight.Speech;
using System.Linq;
using System.Text.RegularExpressions;

namespace Content.Server._Daiquiri.Speech.EntitySystems;

public sealed partial class BarkAccentSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;

    private static readonly IReadOnlyList<string> _barks = new List<string>{
        " Гав!", " ГАВ", " вуф-вуф"
    }.AsReadOnly();

    private static readonly IReadOnlyDictionary<string, string> _specialWords = new Dictionary<string, string>()
    {
        { "ah", "arf" },
        { "Ah", "Arf" },
        { "oh", "oof" },
        { "Oh", "Oof" },
        { "га", "гаф" },
        { "Га", "Гаф" },
        { "угу", "вуф" },
        { "Угу", "Вуф" },
    };

    private static readonly (Regex Regex, string Replacement)[] _specialWordRegexes =
        _specialWords
            .Select(kv => (new Regex(Regex.Escape(kv.Key), RegexOptions.IgnoreCase), kv.Value))
            .ToArray();

    public override void Initialize()
    {
        SubscribeLocalEvent<BarkAccentComponent, AccentGetEvent>(OnAccent);
        SubscribeLocalEvent<BarkAccentComponent, StatusEffectRelayedEvent<AccentGetEvent>>(OnAccentRelayed);
    }

    public SpeechMessage Accentuate(SpeechMessage message)
    {
        foreach (var (regex, repl) in _specialWordRegexes)
        {
            message.Text = regex.Replace(message.Text, m => PreserveCase(m.Value, repl));
            message.Tts = regex.Replace(message.Tts ?? message.Text, m => PreserveCase(m.Value, repl));
        }

        message.Text = message.Text.Replace("!", _random.Pick(_barks))
            .Replace("l", "r")
            .Replace("L", "R")
            .Replace("л", "р")
            .Replace("Л", "Р");

        message.Tts = (message.Tts ?? message.Text).Replace("!", " Гав!");

        return message;
    }

    private static string PreserveCase(string matched, string replacement)
    {
        if (!matched.Any(char.IsLower) && (matched.Length > 1 || replacement.Length == 1))
            return replacement.ToUpperInvariant();

        if (char.IsUpper(matched[0]))
            return char.ToUpperInvariant(replacement[0]) + replacement[1..];

        return replacement;
    }

    private void OnAccent(Entity<BarkAccentComponent> entity, ref AccentGetEvent args)
        => args.Message = Accentuate(args.Message);

    private void OnAccentRelayed(Entity<BarkAccentComponent> entity, ref StatusEffectRelayedEvent<AccentGetEvent> args)
        => args.Args.Message = Accentuate(args.Args.Message);
}
