using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests.Styling;

public static class BaselineTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 10u;

        canvas.Children =
        [
            new FrameStatus(),
            new FlexBox()
            {
                Name  = "box",
                Style = new()
                {
                    Border = new(BORDER_SIZE, 0, Color.Cyan),
                    Size   = new(Unit.Px(100)),
                },
            },
            new FlexBox()
            {
                Name      = "vertical_text",
                InnerText = "Vertical\nText",
                Style     = new()
                {
                    Alignment  = Alignment.Baseline,
                    Border     = new(BORDER_SIZE, 0, Color.Margenta),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 12,
                }
            },
            new FlexBox()
            {
                Name  = "horizontal_container",
                Style = new()
                {
                    Padding        = new(Unit.Px(20)),
                    Border         = new(BORDER_SIZE, 0, Color.Cyan),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    ItemsAlignment = ItemsAlignment.Baseline,
                },
                Children =
                [
                    new Text("Horizontal"),
                    new FlexBox
                    {
                        Name      = "X",
                        InnerText = "X",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE * 2, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 48,
                            Margin     = new(null, Unit.Px(10)),
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Y",
                        InnerText = "Y",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE / 2, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Z",
                        InnerText = "Z",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0,     Color.Blue),
                            Color      = Color.Blue,
                            FontFamily = "Verdana",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Hello",
                        InnerText = "Hello",
                        Style     = new()
                        {
                            Alignment = Alignment.Bottom,
                            Border    = new(BORDER_SIZE, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new(Unit.Px(5))
                        }
                    },
                    new FlexBox
                    {
                        Name      = "World",
                        InnerText = "World!!!",
                        Style     = new()
                        {
                            Alignment = Alignment.Top,
                            Border    = new(BORDER_SIZE, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new(Unit.Px(5))
                        }
                    }
                ]
            },
            new FlexBox()
            {
                Name      = "horizontal_container",
                InnerText = "Horizontal",
                Style     = new()
                {
                    Alignment      = Alignment.Baseline,
                    ItemsAlignment = ItemsAlignment.Baseline,
                    Border         = new(BORDER_SIZE, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 12,
                },
                Children =
                [
                    new Text("Horizontal"),
                    new FlexBox
                    {
                        Name      = "X",
                        InnerText = "X",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Y",
                        InnerText = "Y",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 12,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Z",
                        InnerText = "Z",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Blue),
                            Color      = Color.Blue,
                            FontFamily = "Verdana",
                            FontSize   = 24,
                        }
                    }
                ],
            },
            new FlexBox
            {
                Name  = "horizontal_c_container",
                Style = new()
                {
                    Border         = new(BORDER_SIZE, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    StackDirection = StackDirection.Horizontal,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "horizontal_c_child1",
                        Style = new()
                        {
                            Alignment = Alignment.Left,
                            Border    = new(BORDER_SIZE, 0, Color.Cyan),
                            Margin    = new(Unit.Px(10)),
                            Size      = new(Unit.Px(100)),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "horizontal_c_child2",
                        Style = new()
                        {
                            Alignment = Alignment.Right,
                            Border    = new(BORDER_SIZE, 0, Color.Cyan),
                            Margin    = new(Unit.Px(10)),
                            Size      = new(Unit.Px(100)),
                        }
                    }
                ],
            },
            new FlexBox
            {
                Name  = "vertical_a_container",
                // Text  = "Vertical",
                Style = new()
                {
                    Alignment      = Alignment.Baseline,
                    Border         = new(BORDER_SIZE, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    StackDirection = StackDirection.Vertical,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name      = "X",
                        InnerText = "X",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Y",
                        InnerText = "Y",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name      = "Z",
                        InnerText = "Z",
                        Style     = new()
                        {
                            Border     = new(BORDER_SIZE, 0, Color.Blue),
                            Color      = Color.Blue,
                            FontFamily = "Verdana",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        InnerText = "Hello",
                        Style     = new()
                        {
                            Alignment = Alignment.Center,
                            Border    = new(BORDER_SIZE, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new(Unit.Px(10)),
                        }
                    },
                    new FlexBox
                    {
                        InnerText = "World!!!",
                        Style     = new()
                        {
                            Alignment = Alignment.Left,
                            Border    = new(BORDER_SIZE, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new(Unit.Px(10)),
                        }
                    },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_b_container",
                Style = new()
                {
                    Border         = new(BORDER_SIZE, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    StackDirection = StackDirection.Vertical,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "vertical_b_child1",
                        Style = new()
                        {
                            Size   = new(Unit.Px(10), Unit.Px(200)),
                            Border = new(BORDER_SIZE, 0, Color.Red),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_b_child2",
                        Style = new()
                        {
                            Size   = new(Unit.Px(10), Unit.Px(100)),
                            Border = new(BORDER_SIZE, 0, Color.Green),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_b_child3",
                        Style = new()
                        {
                            Size   = new(Unit.Px(10), Unit.Px(50)),
                            Border = new(BORDER_SIZE, 0, Color.Blue),
                        }
                    },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_c_container",
                Style = new()
                {
                    Alignment      = Alignment.Baseline,
                    Border         = new(BORDER_SIZE, 0, Color.Cyan),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    StackDirection = StackDirection.Vertical,
                },
                Children =
                [
                    new FlexBox { Name = "Vertical", InnerText = "Vertical", Style = new() { Border = new(BORDER_SIZE, 0, Color.Red), Margin = new(null, Unit.Px(0)), FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } },
                    new FlexBox { Name = "X", InnerText = "X", Style = new() { Border = new(BORDER_SIZE, 0, Color.Red),   FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } },
                    new FlexBox { Name = "Y", InnerText = "Y", Style = new() { Border = new(BORDER_SIZE, 0, Color.Green), FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } },
                    new FlexBox { Name = "Z", InnerText = "Z", Style = new() { Border = new(BORDER_SIZE, 0, Color.Blue),  FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } },
                    new FlexBox { InnerText = "Hello",         Style = new() { Border = new(BORDER_SIZE, 0, Color.Margenta), Color = Color.White, Alignment = Alignment.Left,  Margin = new(Unit.Px(5)) } },
                    new FlexBox { InnerText = "World!!!",      Style = new() { Border = new(BORDER_SIZE, 0, Color.Margenta), Color = Color.White, Alignment = Alignment.Right, Margin = new(Unit.Px(5)) } },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_c_container",
                Style = new()
                {
                    Border         = new(BORDER_SIZE, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    StackDirection = StackDirection.Vertical,
                    Alignment      = Alignment.Baseline,
                    ItemsAlignment = ItemsAlignment.Baseline,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "vertical_d_child1",
                        Style = new()
                        {
                            Size      = new(Unit.Px(100)),
                            Margin    = new(Unit.Px(10)),
                            Border    = new(BORDER_SIZE, 0, Color.Cyan),
                            Alignment = Alignment.Top,
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_d_child2",
                        Style = new()
                        {
                            Size      = new(Unit.Px(100)),
                            Margin    = new(Unit.Px(10)),
                            Border    = new(BORDER_SIZE, 0, Color.Cyan),
                            Alignment = Alignment.Bottom,
                        }
                    },
                ],
            }
        ];
    }
}
