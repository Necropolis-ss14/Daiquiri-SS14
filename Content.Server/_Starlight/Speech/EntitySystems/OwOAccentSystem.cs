using System.Linq;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared._Starlight.Speech;
using Content.Shared.Speech;
using Content.Shared.StatusEffectNew;

namespace Content.Server._Starlight.Speech.EntitySystems;

public sealed class OwOAccentSystem : EntitySystem
{
    private static readonly IReadOnlyDictionary<string, string> _specialWords = new Dictionary<string, string>()
    {
        { "you", "wu" },
        { "are", "r" },
        { "hello", "mew" },
        { "love", "luv" },
        { "please", "plez" },
        { "food", "noms" },
        { "cute", "koot" },
        { "now", "meow" },
        { "look", "lookee" },
        { "little", "lil" },
        { "ты", "ти" },
        { "привет", "мяу" },
        { "еда", "нямка" },
        { "пожалуйста", "позязя" },
        { "люблю", "мур" },
        { "милый", "няшный" },
    };

    private static readonly (Regex Regex, string Replacement)[] _specialWordRegexes =
        _specialWords
            .Select(kv => (new Regex(Regex.Escape(kv.Key), RegexOptions.IgnoreCase), kv.Value))
            .ToArray();

    public override void Initialize()
    {
        SubscribeLocalEvent<OwOAccentComponent, AccentGetEvent>(OnAccent);
        SubscribeLocalEvent<OwOAccentComponent, StatusEffectRelayedEvent<AccentGetEvent>>(OnAccentRelayed);
    }

    public SpeechMessage Accentuate(SpeechMessage message)
    {
        foreach (var (regex, repl) in _specialWordRegexes)
        {
            message.Text = regex.Replace(message.Text, m => PreserveCase(m.Value, repl));
            message.Tts = regex.Replace(message.Tts ?? message.Text, m => PreserveCase(m.Value, repl));
        }
        message.Text = message.Text
            .Replace("r", "w").Replace("R", "W")
            .Replace("l", "w").Replace("L", "W")
            .Replace("р", "в").Replace("Р", "В")
            .Replace("л", "в").Replace("Л", "В");

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

    private void OnAccent(Entity<OwOAccentComponent> entity, ref AccentGetEvent args)
        => args.Message = Accentuate(args.Message);

    private void OnAccentRelayed(Entity<OwOAccentComponent> entity, ref StatusEffectRelayedEvent<AccentGetEvent> args)
        => args.Args.Message = Accentuate(args.Args.Message);
}
