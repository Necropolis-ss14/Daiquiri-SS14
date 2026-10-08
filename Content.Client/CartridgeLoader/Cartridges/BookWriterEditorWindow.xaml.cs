using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.CartridgeLoader.Cartridges;

/// <summary>
/// Daiquiri: standalone book editor window. PDA programs are too small for writing.
/// </summary>
public sealed partial class BookWriterEditorWindow : DefaultWindow
{
    public event Action<string, string, string, string, string, string, string, bool, List<string>>? OnSave;

    private string _editId = "";

    private List<string> _pages = new() { "" };
    private int _page;
    private int _variantIdx;
    private int _genreIdx;

    private readonly LineEdit _titleInput = new()
    {
        HorizontalExpand = true,
        PlaceHolder = Loc.GetString("book-writer-title-placeholder"),
    };
    private readonly LineEdit _descInput = new()
    {
        HorizontalExpand = true,
        PlaceHolder = Loc.GetString("book-writer-desc-placeholder"),
    };
    private readonly TextEdit _bodyInput = new()
    {
        HorizontalExpand = true,
        VerticalExpand = true,
        MinHeight = 280,
    };
    private readonly Label _counter = new()
    {
        HorizontalExpand = true,
        HorizontalAlignment = HAlignment.Center,
    };
    private readonly PanelContainer _coverHolder = new()
    {
        VerticalAlignment = VAlignment.Center,
    };
    private readonly Label _variantName = new()
    {
        HorizontalExpand = true,
        VerticalAlignment = VAlignment.Center,
    };
    private readonly Label _genreName = new()
    {
        HorizontalExpand = true,
        VerticalAlignment = VAlignment.Center,
    };
    private readonly Button _prev = new() { Text = "◀" };
    private readonly Button _next = new() { Text = "▶" };
    private readonly Label _status = new()
    {
        HorizontalExpand = true,
        ClipText = true,
        ModulateSelfOverride = new Color(1f, 0.5f, 0.5f),
    };

    public BookWriterEditorWindow()
    {
        MinSize = new Vector2(520, 640);
        Title = Loc.GetString("book-writer-editor-title");

        var variantRow = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            SeparationOverride = 4,
        };
        var variantPrev = new Button { Text = "◀" };
        var variantNext = new Button { Text = "▶" };
        variantPrev.OnPressed += _ => CycleVariant(-1);
        variantNext.OnPressed += _ => CycleVariant(1);
        variantRow.AddChild(variantPrev);
        variantRow.AddChild(_coverHolder);
        variantRow.AddChild(_variantName);
        variantRow.AddChild(variantNext);
        var genreRow = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            SeparationOverride = 4,
        };
        var genrePrev = new Button { Text = "◀" };
        var genreNext = new Button { Text = "▶" };
        genrePrev.OnPressed += _ => CycleGenre(-1);
        genreNext.OnPressed += _ => CycleGenre(1);
        genreRow.AddChild(genrePrev);
        genreRow.AddChild(_genreName);
        genreRow.AddChild(genreNext);

        RefreshCover();
        RefreshGenre();

        var pageBar = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalExpand = true,
        };
        var addPage = new Button { Text = Loc.GetString("book-writer-add-page") };
        _prev.OnPressed += _ => FlipPage(-1);
        _next.OnPressed += _ => FlipPage(1);
        addPage.OnPressed += _ =>
        {
            _pages[_page] = Rope.Collapse(_bodyInput.TextRope);
            _pages.Add("");
            _page = _pages.Count - 1;
            RefreshPage();
        };
        pageBar.AddChild(_prev);
        pageBar.AddChild(_counter);
        pageBar.AddChild(_next);
        pageBar.AddChild(addPage);

        var bar = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalExpand = true,
        };
        var save = new Button { Text = Loc.GetString("book-writer-save-draft"), HorizontalExpand = true };
        save.OnPressed += _ => TrySave(true);
        var publish = new Button { Text = Loc.GetString("book-writer-publish"), HorizontalExpand = true };
        publish.OnPressed += _ => TrySave(false);
        var cancel = new Button { Text = Loc.GetString("book-writer-cancel"), HorizontalExpand = true };
        cancel.OnPressed += _ => Close();
        bar.AddChild(save);
        bar.AddChild(publish);
        bar.AddChild(cancel);

        var root = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            HorizontalExpand = true,
            VerticalExpand = true,
            SeparationOverride = 4,
        };
        root.AddChild(_titleInput);
        root.AddChild(_descInput);
        root.AddChild(new Label { Text = Loc.GetString("book-writer-cover") });
        root.AddChild(variantRow);
        root.AddChild(new Label { Text = Loc.GetString("book-writer-genre") });
        root.AddChild(genreRow);
        root.AddChild(pageBar);
        root.AddChild(_bodyInput);
        root.AddChild(_status);
        root.AddChild(bar);
        Contents.AddChild(root);
        RefreshPage();
    }

    private void FlipPage(int dir)
    {
        _pages[_page] = Rope.Collapse(_bodyInput.TextRope);
        _page = Math.Clamp(_page + dir, 0, _pages.Count - 1);
        RefreshPage();
    }

    private void RefreshCover()
    {
        _variantIdx = (_variantIdx + BookWriterCovers.Variants.Length) % BookWriterCovers.Variants.Length;
        var variant = BookWriterCovers.Variants[_variantIdx];
        _variantName.Text = Loc.GetString(variant.Loc);
        _coverHolder.RemoveAllChildren();
        _coverHolder.AddChild(BookWriterCovers.CoverPanel(variant.Color, variant.Decor, variant.Icon));
    }

    private void CycleVariant(int dir)
    {
        _variantIdx += dir;
        RefreshCover();
    }

    private void RefreshGenre()
    {
        _genreIdx = (_genreIdx + BookWriterCovers.Genres.Length) % BookWriterCovers.Genres.Length;
        _genreName.Text = Loc.GetString(BookWriterCovers.Genres[_genreIdx].Loc);
    }

    private void CycleGenre(int dir)
    {
        _genreIdx += dir;
        RefreshGenre();
    }

    private void RefreshPage()
    {
        _counter.Text = $"{_page + 1} / {_pages.Count}";
        _prev.Disabled = _page <= 0;
        _next.Disabled = _page >= _pages.Count - 1;
        _bodyInput.TextRope = new Rope.Leaf(_pages[_page]);
    }

    /// <summary>
    /// Daiquiri: open with a draft to continue writing it.
    /// </summary>
    public void LoadDraft(
        string id,
        string title,
        string desc,
        string color,
        string decor,
        string icon,
        string genre,
        List<string> pages)
    {
        _editId = id;
        _titleInput.Text = title;
        _descInput.Text = desc;
        _pages = new List<string>(pages);
        _page = 0;
        var variantIdx = Array.FindIndex(BookWriterCovers.Variants,
            v => v.Color == color && v.Decor == decor && v.Icon == icon);
        _variantIdx = variantIdx >= 0 ? variantIdx : 0;
        var genreIdx = Array.FindIndex(BookWriterCovers.Genres, g => g.Value == genre);
        _genreIdx = genreIdx >= 0 ? genreIdx : 0;
        RefreshCover();
        RefreshGenre();
        RefreshPage();
    }

    private void TrySave(bool draft)
    {
        _pages[_page] = Rope.Collapse(_bodyInput.TextRope);
        var title = _titleInput.Text.Trim();
        if (title.Length < 3)
        {
            _status.Text = Loc.GetString("book-writer-err-title");
            return;
        }
        if (title.Length > 64 || _descInput.Text.Trim().Length > 140)
        {
            _status.Text = Loc.GetString("book-writer-err-long");
            return;
        }
        if (!_pages.Any(p => p.Trim().Length > 0))
        {
            _status.Text = Loc.GetString("book-writer-err-empty");
            return;
        }
        if (_pages.Count > 12 || _pages.Any(p => p.Length > 12000))
        {
            _status.Text = Loc.GetString("book-writer-err-long");
            return;
        }
        var variant = BookWriterCovers.Variants[_variantIdx];
        var genre = BookWriterCovers.Genres[_genreIdx];
        OnSave?.Invoke(
            _editId,
            title,
            _descInput.Text.Trim(),
            variant.Color,
            variant.Decor,
            variant.Icon,
            genre.Value,
            draft,
            new List<string>(_pages));
        Close();
    }
}
