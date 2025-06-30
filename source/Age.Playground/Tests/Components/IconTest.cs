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
            FontSize       = 32,
            ItemsAlignment = ItemsAlignment.Baseline,
        };

        canvas.Children =
        [
            new FlexBox
            {
                Style = new Style
                {
                    ItemsAlignment = ItemsAlignment.Baseline,
                },
                Children =
                [
                    new FlexBox
                    {
                        Style    = style,
                        Children =
                        [
                            // new Text("search"),
                            new Icon("search", 32, Color.Red)
                        ]
                    },
                    new FlexBox
                    {
                        Style    = style with { FontSize = 100 },
                        Children =
                        [
                            // new Text("home"),
                            new Icon("home", 100, Color.Green)
                        ]
                    },
                    new FlexBox
                    {
                        Style    = style,
                        Children =
                        [
                            // new Text("close"),
                            new Icon("close", 24, Color.Blue)
                        ]
                    }
                ]
            }
        ];
    }
}
