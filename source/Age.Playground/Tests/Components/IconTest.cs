using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Playground.Tests.Components;

public static class IconTest
{
    public static void Setup(Canvas canvas)
    {
        var style = new Style
        {
            Color          = Color.White,
            FontSize       = 24,
            ItemsAlignment = ItemsAlignment.Baseline,
            Border         = new(1, 0, Color.Cyan)
        };

        canvas.Children =
        [
            new FlexBox
            {
                Style    = style,
                Children =
                [
                    new Text("search"),
                    new Icon("search", color: Color.Red),
                    new Text("home"),
                    new Icon("home",   color: Color.Green) { Style = { Margin = new(10) } },
                    new Text("close"),
                    new Icon("close",  color: Color.Blue),
                ]
            }
        ];
    }
}
