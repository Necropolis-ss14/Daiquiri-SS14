using System.Text;
using System.Text.RegularExpressions;

namespace Content.Server._Starlight.TextToSpeech;

public static partial class NumberConverter
{
    private static readonly string[] _unitsMasculine =
    [
        "", "один", "два", "три", "четыре", "пять", "шесть",
        "семь", "восемь", "девять", "десять", "одиннадцать",
        "двенадцать", "тринадцать", "четырнадцать", "пятнадцать",
        "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
    ];

    private static readonly string[] _unitsFeminine =
    [
        "", "одна", "две", "три", "четыре", "пять", "шесть",
        "семь", "восемь", "девять", "десять", "одиннадцать",
        "двенадцать", "тринадцать", "четырнадцать", "пятнадцать",
        "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
    ];

    private static readonly string[] _tens =
    [
        "", "десять", "двадцать", "тридцать", "сорок", "пятьдесят",
        "шестьдесят", "семьдесят", "восемьдесят", "девяносто"
    ];

    private static readonly string[] _hundreds =
    [
        "", "сто", "двести", "триста", "четыреста", "пятьсот",
        "шестьсот", "семьсот", "восемьсот", "девятьсот"
    ];

    private static readonly string[] _digits =
    ["ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"];

    public static string Convert(string numberStr)
    {
        numberStr = numberStr.Replace(",", "");

        if (numberStr.Contains('.'))
        {
            var parts = numberStr.Split('.', 2);
            var integerPart = string.IsNullOrEmpty(parts[0]) || parts[0] == "-"
                ? "ноль"
                : NumberToText(long.Parse(parts[0]));

            var decimalPart = DecimalToText(parts[1]);

            return $"{integerPart} запятая {decimalPart}";
        }

        return NumberToText(long.Parse(numberStr));
    }

    private static string DecimalToText(string digits)
    {
        var result = new StringBuilder();

        foreach (var c in digits)
        {
            if (char.IsDigit(c))
            {
                if (result.Length > 0)
                    result.Append(' ');
                result.Append(_digits[c - '0']);
            }
        }

        return result.ToString();
    }

    public static string NumberToText(long number)
    {
        if (number == 0)
            return "ноль";

        if (number < 0)
            return "минус " + NumberToText(-number);

        var words = new StringBuilder();

        // (scale, one, few, many, feminine)
        var scales = new (long scale, string one, string few, string many, bool feminine)[]
        {
            (1_000_000_000_000L, "триллион", "триллиона", "триллионов", false),
            (1_000_000_000L, "миллиард", "миллиарда", "миллиардов", false),
            (1_000_000L, "миллион", "миллиона", "миллионов", false),
            (1_000L, "тысяча", "тысячи", "тысяч", true),
        };

        foreach (var (scale, one, few, many, feminine) in scales)
        {
            if (number < scale)
                continue;

            var count = number / scale;
            number %= scale;

            words.Append(ConvertChunk((int)count, feminine));
            words.Append(' ');
            words.Append(Plural(count, one, few, many));
            words.Append(' ');
        }

        if (number > 0)
            words.Append(ConvertChunk((int)number, false));

        return words.ToString().Trim();
    }

    private static string Plural(long n, string one, string few, string many)
    {
        var n10 = n % 10;
        var n100 = n % 100;
        if (n10 == 1 && n100 != 11)
            return one;
        if (n10 >= 2 && n10 <= 4 && (n100 < 12 || n100 > 14))
            return few;
        return many;
    }

    private static string ConvertChunk(int number, bool feminine)
    {
        var result = new StringBuilder();
        var units = feminine ? _unitsFeminine : _unitsMasculine;

        var hundreds = number / 100;
        var tensUnits = number % 100;

        if (hundreds > 0)
        {
            result.Append(_hundreds[hundreds]);
            if (tensUnits > 0)
                result.Append(' ');
        }

        if (tensUnits > 0)
        {
            if (tensUnits < 20)
            {
                result.Append(units[tensUnits]);
            }
            else
            {
                var tens = tensUnits / 10;
                var rest = tensUnits % 10;

                result.Append(_tens[tens]);
                if (rest > 0)
                    result.Append(' ' + units[rest]);
            }
        }

        return result.ToString();
    }

    [GeneratedRegex(@"-?\d{1,3}(,\d{3})*(\.\d+)?|-?\d+(\.\d+)?")]
    public static partial Regex NumberPattern();
}
