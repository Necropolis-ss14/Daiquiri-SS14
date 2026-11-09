using System.Linq;
using Content.Client.Parallax;
using Content.Client.Parallax.Data;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared._Daiquiri.Planets;
using Content.Shared.Atmos;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._Daiquiri.Planets.UI;

/// <summary>
/// One planet on the flight console: an animated strip of its surface, a short pitch and the
/// readouts that decide whether the trip is survivable, plus the button that prints its disk.
/// </summary>
public sealed class PlanetCard : PanelContainer
{
    public const float CardWidth = 380f;
    public const float CardHeight = 340f;

    /// <summary>
    /// How much of the card the pitch may claim at its very least. Without a cap the longest
    /// description sets the card's minimum height and pushes the jump button off the bottom.
    /// </summary>
    private const float DescriptionMaxHeight = 72f;

    private const float BannerHeight = 92f;

    private static readonly Color CardBackground = Color.FromHex("#141A21");
    private static readonly Color MutedText = Color.FromHex("#AEBAC4");
    private static readonly Color ColdText = Color.FromHex("#7FD4FF");
    private static readonly Color MildText = Color.FromHex("#CFE6F2");
    private static readonly Color HotText = Color.FromHex("#FF8A5B");
    private static readonly Color GoodText = Color.FromHex("#6FD68A");
    private static readonly Color BadText = Color.FromHex("#FF7A7A");

    /// <summary>
    /// Gases that make an atmosphere unbreathable no matter how much oxygen is in it.
    /// </summary>
    private static readonly HashSet<Gas> ToxicGases = new()
    {
        Gas.Plasma,
        Gas.Tritium,
        Gas.Ammonia,
        Gas.NitrousOxide,
        Gas.Zauker,
        Gas.BZ,
        Gas.Frezon,
        Gas.Halon,
        Gas.Hydrogen,
    };

    public event Action<string>? PrintPressed;

    public string PlanetId { get; }

    // Built by MakeBody, which the constructor calls before anything can ask for the card's state.
    private Button _printButton = default!;

    // The banner's parallax, kept so its scale can follow the width the card ends up with.
    private ParallaxControl? _banner;

    public PlanetCard(PlanetDestinationPrototype planet)
    {
        PlanetId = planet.ID;

        var prototype = IoCManager.Resolve<IPrototypeManager>();

        SetWidth = CardWidth;
        SetHeight = CardHeight;
        PanelOverride = new StyleBoxFlat
        {
            BackgroundColor = CardBackground,
            BorderColor = planet.Accent.WithAlpha(0.55f),
            BorderThickness = new Thickness(1f),
        };

        var root = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            SeparationOverride = 0,
        };

        AddChild(root);
        root.AddChild(MakeBanner(planet, prototype));
        UpdateBannerScale();
        root.AddChild(new PanelContainer
        {
            SetHeight = 3f,
            PanelOverride = new StyleBoxFlat(planet.Accent),
        });
        root.AddChild(MakeBody(planet));
    }

    protected override void Resized()
    {
        base.Resized();
        UpdateBannerScale();
    }

    /// <summary>
    /// Redraws the banner at the width the card was given. The parallax lays its layers out for a
    /// 1920 wide screen, so the scale has to undo that for the surface to come out at its native
    /// size - and it is taken from the width the card ends up with rather than from
    /// <see cref="CardWidth"/>, so the strip stays right if the cards are ever sized differently.
    /// </summary>
    private void UpdateBannerScale()
    {
        if (_banner == null)
            return;

        var width = Size.X > 0f ? Size.X : CardWidth;
        var scale = 1920f / width;

        if (MathHelper.CloseTo(_banner.ScaleX, scale))
            return;

        _banner.ScaleX = scale;
        _banner.ScaleY = scale;
    }

    /// <summary>
    /// The strip of surface on top of the card. It is only a picture - the planet itself is
    /// generated on the server once the disk is printed - so a missing parallax is not fatal.
    /// </summary>
    private Control MakeBanner(PlanetDestinationPrototype planet, IPrototypeManager prototype)
    {
        var banner = new Control
        {
            SetHeight = BannerHeight,
            RectClipContent = true,
        };

        var parallax = planet.Parallax;
        if (parallax != null && prototype.HasIndex<ParallaxPrototype>(parallax))
        {
            _banner = new ParallaxControl { ParallaxPrototype = parallax };
            banner.AddChild(_banner);
        }

        banner.AddChild(new PanelContainer
        {
            PanelOverride = new StyleBoxFlat(planet.Accent.WithAlpha(0.18f)),
        });

        var nameStrip = new PanelContainer
        {
            VerticalAlignment = VAlignment.Bottom,
            PanelOverride = new StyleBoxFlat(Color.Black.WithAlpha(0.5f)),
        };

        nameStrip.AddChild(new Label
        {
            Text = Loc.GetString(planet.Name),
            StyleClasses = { StyleClass.LabelHeading },
            Margin = new Thickness(10f, 3f, 10f, 3f),
        });

        banner.AddChild(nameStrip);
        return banner;
    }

    private Control MakeBody(PlanetDestinationPrototype planet)
    {
        var body = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            SeparationOverride = 6,
            VerticalExpand = true,
            Margin = new Thickness(12f, 8f, 12f, 10f),
        };

        // The pitch is the only part that can grow, so it takes whatever room is left and clips
        // rather than pushing the jump button off the bottom of the card. Its minimum height is
        // capped for the same reason: a long description would otherwise set the card's minimum
        // height on its own, since a control measures to its tallest child.
        var descriptionBox = new Control
        {
            VerticalExpand = true,
            MinHeight = 4f,
            MaxHeight = DescriptionMaxHeight,
            RectClipContent = true,
        };

        if (!string.IsNullOrEmpty(planet.Description))
        {
            var description = new RichTextLabel();
            description.SetMessage(Loc.GetString(planet.Description), MutedText);
            descriptionBox.AddChild(description);
        }

        body.AddChild(descriptionBox);

        body.AddChild(new HLine { Color = planet.Accent.WithAlpha(0.35f), Thickness = 1f, Margin = new Thickness(0f, 2f, 0f, 2f) });

        var temperature = new Label
        {
            Text = $"{planet.Temperature:0.#} °C",
            FontColorOverride = planet.Temperature switch
            {
                <= 0f => ColdText,
                >= 40f => HotText,
                _ => MildText,
            },
        };

        var breathable = IsBreathable(planet);
        var atmosphere = new Label
        {
            Text = Loc.GetString(breathable ? "planet-console-breathable" : "planet-console-unbreathable"),
            FontColorOverride = breathable ? GoodText : BadText,
        };

        var light = new PanelContainer
        {
            SetWidth = 28f,
            SetHeight = 12f,
            VerticalAlignment = VAlignment.Center,
            PanelOverride = new StyleBoxFlat(planet.MapLight),
        };

        body.AddChild(MakeStat("planet-console-stat-temperature", temperature));
        body.AddChild(MakeStat("planet-console-stat-atmosphere", atmosphere));
        body.AddChild(MakeStat("planet-console-stat-light", light));

        _printButton = new Button
        {
            Text = Loc.GetString("planet-console-print"),
            HorizontalExpand = true,
        };

        _printButton.OnPressed += _ => PrintPressed?.Invoke(PlanetId);
        body.AddChild(_printButton);

        return body;
    }

    /// <summary>
    /// Locks the card while the console is busy: either this very planet is being printed, or
    /// the console is still recharging from the last disk.
    /// </summary>
    public void SetState(bool printing, bool blocked)
    {
        _printButton.Disabled = printing || blocked;
        _printButton.Text = Loc.GetString(printing ? "planet-console-printing" : "planet-console-print");
    }

    private static BoxContainer MakeStat(string key, Control value)
    {
        // Every row is given the same height and centred contents so the three readouts line up
        // with each other even though one of them is a colour swatch instead of text.
        var row = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            SeparationOverride = 6,
            MinHeight = 22f,
        };

        var label = new Label
        {
            Text = Loc.GetString(key),
            StyleClasses = { StyleClass.LabelKeyText },
            HorizontalExpand = true,
            VerticalAlignment = VAlignment.Center,
        };

        value.VerticalAlignment = VAlignment.Center;

        row.AddChild(label);
        row.AddChild(value);
        return row;
    }

    /// <summary>
    /// A planet counts as breathable when the air is mostly oxygen and free of anything that
    /// would make a maskless stroll a short one.
    /// </summary>
    private static bool IsBreathable(PlanetDestinationPrototype planet)
    {
        var total = planet.Atmosphere.Values.Sum();
        if (total <= 0f)
            return false;

        if (planet.Atmosphere.Keys.Any(ToxicGases.Contains))
            return false;

        return planet.Atmosphere.GetValueOrDefault(Gas.Oxygen) / total >= 0.16f;
    }
}
