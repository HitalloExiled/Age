using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.StoryBook.Pages;

public class IconPage : Page
{
    public override string NodeName => nameof(IconPage);
    public override string Title    => "Icon";

    public IconPage()
    {
        var styleSheet = new StyleSheet
        {
            Base = new()
            {
                FontSize    = 30,
                Color       = Color.White,
                FontFeature = FontFeature.Liga,
                FontFamily  = "Material Icons",
            },
            FontFaces =
            {
                ["Material Icons"] = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts", "MaterialIcons-Regular.ttf")
            }
        };

        this.Children =
        [
            new FlexBox
            {
                Text       = "search",
                StyleSheet = styleSheet,
            },
            new FlexBox
            {
                Text       = "home",
                StyleSheet = styleSheet,
            },
            new FlexBox
            {
                Text       = "close",
                StyleSheet = styleSheet,
            }
        ];
    }
}
