using Content.Server._Starlight.TextToSpeech;
using NUnit.Framework;

namespace Content.Tests.Server.TextToSpeech;

/// <summary>
/// Unit tests verifying the text cleaning and normalization behavior in TTSSystem.
/// </summary>
[TestFixture]
public sealed class TTSTests
{
    /// <summary>
    /// Verifies that standard ASCII apostrophes (used in contractions, accents, or plurals) are preserved.
    /// </summary>
    [Test]
    public void TestCleanTextPreservesApostrophes()
    {
        // Contractions
        Assert.That(TTSSystem.CleanText("I'm here"), Is.EqualTo("и'м хере"));
        Assert.That(TTSSystem.CleanText("I'll do it"), Is.EqualTo("и'лл до ит"));
        Assert.That(TTSSystem.CleanText("don't stop"), Is.EqualTo("дон'т стоп"));
        Assert.That(TTSSystem.CleanText("won't stop"), Is.EqualTo("вон'т стоп"));

        // Accent ending/beginning and plural apostrophes
        Assert.That(TTSSystem.CleanText("thinkin'"), Is.EqualTo("тинкин'"));
        Assert.That(TTSSystem.CleanText("'ello"), Is.EqualTo("'елло"));
        Assert.That(TTSSystem.CleanText("Chris' coat"), Is.EqualTo("чрис' cоат"));
    }

    /// <summary>
    /// Verifies that unicode smart quotes (both single quotes and smart apostrophes) are normalized to standard ASCII apostrophes.
    /// </summary>
    [Test]
    public void TestCleanTextNormalizesSmartQuotes()
    {
        // Smart quote normalization
        Assert.That(TTSSystem.CleanText("I’m here"), Is.EqualTo("и'м хере"));
        Assert.That(TTSSystem.CleanText("I’ll do it"), Is.EqualTo("и'лл до ит"));
        Assert.That(TTSSystem.CleanText("don’t stop"), Is.EqualTo("дон'т стоп"));
        Assert.That(TTSSystem.CleanText("‘ello"), Is.EqualTo("'елло"));
        Assert.That(TTSSystem.CleanText("Chris’ coat"), Is.EqualTo("чрис' cоат"));
    }

    /// <summary>
    /// Verifies that formatting tags (like BBCode) and non-alphanumeric/non-punctuation characters are stripped.
    /// </summary>
    [Test]
    public void TestCleanTextStripsInvalidCharacters()
    {
        // Standard stripping behavior
        Assert.That(TTSSystem.CleanText("[color=red]Hello[/color] world!"), Is.EqualTo("хелло ворлд!"));
        Assert.That(TTSSystem.CleanText("Hello @#$% world!"), Is.EqualTo("хелло  ворлд!"));
    }
}
