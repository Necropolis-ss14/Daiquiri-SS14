namespace Content.Shared.CartridgeLoader.Cartridges;

/// <summary>
/// Daiquiri: printable book cover variants. Single source for client preview and server printing.
/// </summary>
public static class BookCoverVariants
{
    public static readonly (string Loc, string Color, string Decor, string Icon, string Proto)[] Variants =
    {
        ("book-writer-variant-archive", "#1134A6", "decor_spine", "book_icon", "BookPrintedArchive"),
        ("book-writer-variant-scarlet", "#A61111", "decor_diagonal", "icon_eye", "BookPrintedScarlet"),
        ("book-writer-variant-herbal", "#117A11", "decor_bottom", "icon_tree", "BookPrintedHerbal"),
        ("book-writer-variant-grimoire", "#6A117A", "decor_middle", "icon_magic", "BookPrintedGrimoire"),
        ("book-writer-variant-codex", "#222222", "decor_vertical_middle", "icon_law", "BookPrintedCodex"),
        ("book-writer-variant-folio", "#d4af37", "decor_spine", "icon_temple", "BookPrintedFolio"),
        ("book-writer-variant-bestiary", "#3a5b2f", "decor_wingette", "icon_possum", "BookPrintedBestiary"),
        ("book-writer-variant-atlas", "#1a4a6b", "decor_wingette_flat", "icon_planet", "BookPrintedAtlas"),
        ("book-writer-variant-chronicle", "#7a2f1a", "decor_wingette_circle", "icon_time", "BookPrintedChronicle"),
        ("book-writer-variant-songs", "#704010", "decor_bottom", "icon_text", "BookPrintedSongs"),
        ("book-writer-variant-necro", "#3d2b1f", "decor_middle", "icon_skull", "BookPrintedNecro"),
        ("book-writer-variant-almanac", "#1f5b6b", "decor_diagonal", "icon_stars", "BookPrintedAlmanac"),
    };
}
