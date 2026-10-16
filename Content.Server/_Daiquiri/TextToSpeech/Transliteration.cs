using System.Text;

namespace Content.Server._Daiquiri.TextToSpeech;

/// <summary>
/// Transliterates Latin script to Cyrillic so the Russian TTS voices read
/// everything Russian-style instead of English. Applied in TTSSystem.CleanText.
/// </summary>
public static class Transliteration
{
    private static readonly (string From, string To)[] _digraphs =
    [
        ("shch", "щ"), ("sch", "щ"),
        ("zh", "ж"), ("kh", "х"), ("ts", "ц"),
        ("ch", "ч"), ("sh", "ш"), ("ph", "ф"), ("th", "т"),
        ("ya", "я"), ("yu", "ю"), ("yo", "йо"), ("ye", "е"), ("yi", "и"),
        ("ay", "ай"), ("ey", "ей"), ("oy", "ой"),
        ("aw", "ав"), ("ew", "ью"),
        ("oo", "у"), ("ee", "и"), ("ou", "ау"),
    ];

    private static readonly Dictionary<char, string> _singles = new()
    {
        ['a'] = "а", ['b'] = "б", ['d'] = "д", ['e'] = "е",
        ['f'] = "ф", ['g'] = "г", ['h'] = "х", ['i'] = "и",
        ['j'] = "дж", ['k'] = "к", ['l'] = "л", ['m'] = "м",
        ['n'] = "н", ['o'] = "о", ['p'] = "п", ['q'] = "к",
        ['r'] = "р", ['s'] = "с", ['t'] = "т", ['u'] = "у",
        ['v'] = "в", ['w'] = "в", ['x'] = "кс", ['y'] = "й",
        ['z'] = "з",
    };

    public static string Transliterate(string text)
    {
        var result = new StringBuilder(text.Length + 16);
        var i = 0;

        while (i < text.Length)
        {
            var c = text[i];
            if (!IsLatin(c))
            {
                result.Append(c);
                i++;
                continue;
            }

            var matched = false;
            foreach (var (from, to) in _digraphs)
            {
                if (StartsWithIgnoreCase(text, i, from))
                {
                    result.Append(to);
                    i += from.Length;
                    matched = true;
                    break;
                }
            }

            if (matched)
                continue;

            var lower = char.ToLowerInvariant(c);
            if (lower == 'c' && i + 1 < text.Length && "eiy".Contains(char.ToLowerInvariant(text[i + 1])))
            {
                result.Append('с');
            }
            else if (_singles.TryGetValue(lower, out var mapped))
            {
                result.Append(mapped);
            }
            else
            {
                result.Append(c);
            }

            i++;
        }

        return result.ToString();
    }

    private static bool IsLatin(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    private static bool StartsWithIgnoreCase(string text, int index, string value)
    {
        if (index + value.Length > text.Length)
            return false;
        for (var k = 0; k < value.Length; k++)
        {
            if (char.ToLowerInvariant(text[index + k]) != value[k])
                return false;
        }
        return true;
    }
}
