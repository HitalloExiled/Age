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
        var style = new Style
        {
            FontSize   = 30,
            Color      = Color.White,
            FontFamily = "Material Icons Outlined",
        };

        this.Children =
        [
            new FlexBox
            {
                Text  = char.ConvertFromUtf32(0xe8b6),
                Style = style,
            },
            new FlexBox
            {
                Text  = char.ConvertFromUtf32(0xea0b),
                Style = style,
            },
            new FlexBox
            {
                Text  = char.ConvertFromUtf32(0xe80d),
                Style = style,
            },
            new FlexBox
            {
                Text  = char.ConvertFromUtf32(0xe0be),
                Style = style,
            }
        ];
    }
}
