using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed partial class BookWriterUi : UIFragment
{
    private BookWriterUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new BookWriterUiFragment();
        _fragment.OnSaveBook += (id, title, desc, color, decor, icon, genre, draft, pages) =>
            SendBookMessage(BookWriterUiAction.Save, id, title, desc, color, decor, icon, genre, draft, pages, userInterface);
        _fragment.OnDeleteBook += id =>
            SendBookMessage(BookWriterUiAction.Delete, id, "", "", "", "", "", "", false, new List<string>(), userInterface);
        _fragment.OnPrintBook += id =>
            SendBookMessage(BookWriterUiAction.Print, id, "", "", "", "", "", "", false, new List<string>(), userInterface);
        // Daiquiri: identify the viewer so the server sends their drafts.
        SendBookMessage(BookWriterUiAction.Hello, "", "", "", "", "", "", "", false, new List<string>(), userInterface);
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not BookWriterUiState bookState)
            return;

        _fragment?.UpdateState(bookState.Books, bookState.PrintCooldown);
    }

    private void SendBookMessage(
        BookWriterUiAction action,
        string bookId,
        string title,
        string desc,
        string color,
        string decor,
        string icon,
        string genre,
        bool draft,
        List<string> pages,
        BoundUserInterface userInterface)
    {
        var bookMessage = new BookWriterUiMessageEvent(action, bookId, title, desc, color, decor, icon, genre, draft, pages);
        var message = new CartridgeUiMessage(bookMessage);
        userInterface.SendMessage(message);
    }
}
