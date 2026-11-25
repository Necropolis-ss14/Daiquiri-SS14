using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client._Daiquiri.Xeno.QueenHiveEye;

public sealed class XenoQueenHiveWindow : DefaultWindow
{
    public BoxContainer List { get; } = new()
    {
        Orientation = BoxContainer.LayoutOrientation.Vertical,
        SeparationOverride = 4,
        MinWidth = 220,
    };

    public XenoQueenHiveWindow()
    {
        Title = Loc.GetString("xeno-queen-hive-window-title");
        MinSize = new Vector2(260, 120);
        Contents.AddChild(List);
    }
}
