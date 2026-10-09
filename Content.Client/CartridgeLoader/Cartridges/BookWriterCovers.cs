using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.CartridgeLoader.Cartridges;

/// <summary>
/// Daiquiri: shared cover presets and cover preview for PDA books.
/// </summary>
public static class BookWriterCovers
{
    /// <summary>
    /// Daiquiri: cover variants (single source in shared).
    /// </summary>
    public static (string Loc, string Color, string Decor, string Icon, string Proto)[] Variants =>
        Content.Shared.CartridgeLoader.Cartridges.BookCoverVariants.Variants;
    public static readonly (string Loc, string Value)[] Genres =
    {
        ("book-writer-genre-other", "other"),
        ("book-writer-genre-scifi", "scifi"),
        ("book-writer-genre-horror", "horror"),
        ("book-writer-genre-detective", "detective"),
        ("book-writer-genre-romance", "romance"),
        ("book-writer-genre-guide", "guide"),
        ("book-writer-genre-history", "history"),
        ("book-writer-genre-poetry", "poetry"),
        ("book-writer-genre-manga", "manga"),
        ("book-writer-genre-comics", "comics"),
        ("book-writer-genre-hentai", "hentai"),
        ("book-writer-genre-erotica", "erotica"),
        ("book-writer-genre-fantasy", "fantasy"),
        ("book-writer-genre-thriller", "thriller"),
        ("book-writer-genre-mystery", "mystery"),
        ("book-writer-genre-adventure", "adventure"),
        ("book-writer-genre-drama", "drama"),
        ("book-writer-genre-comedy", "comedy"),
        ("book-writer-genre-tragedy", "tragedy"),
        ("book-writer-genre-fable", "fable"),
        ("book-writer-genre-fairytale", "fairytale"),
        ("book-writer-genre-legend", "legend"),
        ("book-writer-genre-myth", "myth"),
        ("book-writer-genre-biography", "biography"),
        ("book-writer-genre-autobiography", "autobiography"),
        ("book-writer-genre-diary", "diary"),
        ("book-writer-genre-textbook", "textbook"),
        ("book-writer-genre-science", "science"),
        ("book-writer-genre-fanfic", "fanfic"),
    };

    private const string DecorTint = "#e8dcc0";

    /// <summary>
    /// Daiquiri: book cover like in-game books — colored cover, decor stripe, icon on top.
    /// </summary>
    public static PanelContainer CoverPanel(string colorHex, string decor, string icon)
    {
        // Daiquiri: old records lack these — fall back to defaults.
        if (string.IsNullOrEmpty(colorHex))
            colorHex = "#1134A6";
        if (string.IsNullOrEmpty(decor))
            decor = "decor_spine";
        if (string.IsNullOrEmpty(icon))
            icon = "book_icon";
        var panel = new PanelContainer
        {
            MinSize = new System.Numerics.Vector2(40, 48),
        };
        try
        {
            var coverTex = CoverTexture("cover_base");
            if (coverTex != null)
            {
                panel.AddChild(new TextureRect
                {
                    Texture = coverTex,
                    Stretch = TextureRect.StretchMode.Scale,
                    ModulateSelfOverride = Color.FromHex(colorHex),
                });
            }
        }
        catch
        {
            // Keep whatever rendered.
        }
        if (!string.IsNullOrEmpty(decor))
        {
            var decorTex = CoverTexture(decor);
            if (decorTex != null)
            {
                panel.AddChild(new TextureRect
                {
                    Texture = decorTex,
                    Stretch = TextureRect.StretchMode.Scale,
                    ModulateSelfOverride = Color.FromHex(DecorTint),
                });
            }
        }
        var iconTex = CoverTexture(icon);
        if (iconTex != null)
        {
            panel.AddChild(new TextureRect
            {
                Texture = iconTex,
                Stretch = TextureRect.StretchMode.KeepAspectCentered,
            });
        }
        return panel;
    }

    public static Texture? CoverTexture(string icon)
    {
        try
        {
            var cache = IoCManager.Resolve<IResourceCache>();
            var sys = IoCManager.Resolve<IEntitySystemManager>()
                .GetEntitySystem<Robust.Client.GameObjects.SpriteSystem>();
            return sys.Frame0(new SpriteSpecifier.Rsi(
                new ResPath("Objects/Misc/books.rsi"), icon));
        }
        catch
        {
            return null;
        }
    }
}
