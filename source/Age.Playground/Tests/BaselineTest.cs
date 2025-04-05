using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class BaselineTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        canvas.Children =
        [
            new FrameStatus(),
            new FlexBox()
            {
                Name  = "box",
                Style = new()
                {
                    Border = new(borderSize, 0, Color.Cyan),
                    Size   = new((Pixel)100),
                },
            },
            new FlexBox()
            {
                Name  = "vertical_text",
                Text  = "Vertical\nText",
                Style = new()
                {
                    Alignment  = AlignmentKind.Baseline,
                    Border     = new(borderSize, 0, Color.Margenta),
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
                    Padding        = new((Pixel)20),
                    Border         = new(borderSize, 0, Color.Cyan),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 24,
                    ItemsAlignment = ItemsAlignmentKind.Baseline,
                },
                Children =
                [
                    new Text("Horizontal"),
                    new FlexBox
                    {
                        Name = "X",
                        Text = "X",
                        Style = new()
                        {
                            Border     = new(borderSize * 2, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 48,
                            Hidden     = false,
                            Margin     = new(null, (Pixel)10),
                        }
                    },
                    new FlexBox
                    {
                        Name = "Y",
                        Text = "Y",
                        Style = new()
                        {
                            Border     = new(borderSize / 2, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Z",
                        Text = "Z",
                        Style = new()
                        {
                            Border     = new(borderSize, 0,     Color.Blue),
                            Color      = Color.Blue,
                            FontFamily = "Verdana",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Hello",
                        Text = "Hello",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Bottom,
                            Border    = new(borderSize, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new((Pixel)5)
                        }
                    },
                    new FlexBox
                    {
                        Name  = "World",
                        Text  = "World!!!",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Top,
                            Border    = new(borderSize, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new((Pixel)5)
                        }
                    }
                ]
            },
            new FlexBox()
            {
                Name  = "horizontal_container",
                Text  = "Horizontal",
                Style = new()
                {
                    Alignment      = AlignmentKind.Baseline,
                    ItemsAlignment = ItemsAlignmentKind.Baseline,
                    Border         = new(borderSize, 0, Color.Margenta),
                    Color          = Color.White,
                    FontFamily     = "Impact",
                    FontSize       = 12,
                },
                Children =
                [
                    new Text("Horizontal"),
                    new FlexBox
                    {
                        Name = "X",
                        Text = "X",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Y",
                        Text = "Y",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 12,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Z",
                        Text = "Z",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Blue),
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
                    Border     = new(borderSize, 0, Color.Margenta),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 24,
                    Stack      = StackKind.Horizontal,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "horizontal_c_child1",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Left,
                            Border    = new(borderSize, 0, Color.Cyan),
                            Margin    = new((Pixel)10),
                            Size      = new((Pixel)100),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "horizontal_c_child2",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Right,
                            Border    = new(borderSize, 0, Color.Cyan),
                            Margin    = new((Pixel)10),
                            Size      = new((Pixel)100),
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
                    Alignment  = AlignmentKind.Baseline,
                    Border     = new(borderSize, 0, Color.Margenta),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 24,
                    Stack      = StackKind.Vertical,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name = "X",
                        Text = "X",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Red),
                            Color      = Color.Red,
                            FontFamily = "Helvetica Neue",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Y",
                        Text = "Y",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Green),
                            Color      = Color.Green,
                            FontFamily = "Lucida Console",
                            FontSize   = 24,
                        }
                    },
                    new FlexBox
                    {
                        Name = "Z",
                        Text = "Z",
                        Style = new()
                        {
                            Border     = new(borderSize, 0, Color.Blue),
                            Color      = Color.Blue,
                            FontFamily = "Verdana",
                            FontSize   = 48,
                        }
                    },
                    new FlexBox
                    {
                        Text = "Hello",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Center,
                            Border    = new(borderSize, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new((Pixel)10),
                        }
                    },
                    new FlexBox
                    {
                        Text = "World!!!",
                        Style = new()
                        {
                            Alignment = AlignmentKind.Left,
                            Border    = new(borderSize, 0, Color.Margenta),
                            Color     = Color.White,
                            Margin    = new((Pixel)10),
                        }
                    },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_b_container",
                Style = new()
                {
                    Border     = new(borderSize, 0, Color.Margenta),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 24,
                    Stack      = StackKind.Vertical,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name = "vertical_b_child1",
                        Style = new()
                        {
                            Size   = new((Pixel)10, (Pixel)200),
                            Border = new(borderSize, 0, Color.Red),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_b_child2",
                        Style = new()
                        {
                            Size   = new((Pixel)10, (Pixel)100),
                            Border = new(borderSize, 0, Color.Green),
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_b_child3",
                        Style = new()
                        {
                            Size   = new((Pixel)10, (Pixel)50),
                            Border = new(borderSize, 0, Color.Blue),
                        }
                    },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_c_container",
                Style = new()
                {
                    Alignment  = AlignmentKind.Baseline,
                    Border     = new(borderSize, 0, Color.Cyan),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 24,
                    Stack      = StackKind.Vertical,
                },
                Children =
                [
                    new FlexBox { Name = "Vertical", Text = "Vertical", Style = new() { Border = new(borderSize, 0, Color.Red), Margin = new(null, (Pixel)0), FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } },
                    new FlexBox { Name = "X", Text = "X", Style = new() { Border = new(borderSize, 0, Color.Red),   FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } },
                    new FlexBox { Name = "Y", Text = "Y", Style = new() { Border = new(borderSize, 0, Color.Green), FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } },
                    new FlexBox { Name = "Z", Text = "Z", Style = new() { Border = new(borderSize, 0, Color.Blue),  FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } },
                    new FlexBox { Text = "Hello",         Style = new() { Border = new(borderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentKind.Left,  Margin = new((Pixel)5) } },
                    new FlexBox { Text = "World!!!",      Style = new() { Border = new(borderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentKind.Right, Margin = new((Pixel)5) } },
                ],
            },
            new FlexBox
            {
                Name  = "vertical_c_container",
                Style = new()
                {
                    Border     = new(borderSize, 0, Color.Margenta),
                    Color      = Color.White,
                    FontFamily = "Impact",
                    FontSize   = 24,
                    Stack      = StackKind.Vertical,
                    Alignment      = AlignmentKind.Baseline,
                    ItemsAlignment = ItemsAlignmentKind.Baseline,
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "vertical_d_child1",
                        Style = new()
                        {
                            Size      = new((Pixel)100),
                            Margin    = new((Pixel)10),
                            Border    = new(borderSize, 0, Color.Cyan),
                            Alignment = AlignmentKind.Top,
                        }
                    },
                    new FlexBox
                    {
                        Name  = "vertical_d_child2",
                        Style = new()
                        {
                            Size      = new((Pixel)100),
                            Margin    = new((Pixel)10),
                            Border    = new(borderSize, 0, Color.Cyan),
                            Alignment = AlignmentKind.Bottom,
                        }
                    },
                ],
            }
        ];
    }
}
