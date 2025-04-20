using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class BoxModelTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        canvas.Children =
        [
            new FlexBox
            {
                Name  = "BoxModel",
                Style = new()
                {
                    Border    = new(borderSize, 0, Color.Red),
                    Size      = new((Percentage)100),
                    BoxSizing = BoxSizing.Border
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "StackL",
                        Style = new()
                        {
                            StackDirection = StackDirection.Vertical,
                            Border         = new(borderSize, 0, Color.Green),
                            Size           = new((Percentage)50, (Percentage)100),
                        },
                        Children =
                        [
                            new FlexBox
                            {
                                Text  = "This elements should be stacked vertically...",
                                Name  = "StackText",
                                Style = new()
                                {
                                    Alignment = Alignment.Center,
                                    Color     = Color.White,
                                    Border    = new(10, 0, Color.Margenta),
                                }
                            },
                            new FlexBox
                            {
                                Name  = "BoxAL",
                                Style = new()
                                {
                                    Color  = Color.White,
                                    Border = new(borderSize, 0, Color.Cyan),
                                    Size   = new((Percentage)100, (Percentage)50),
                                }
                            },
                            new FlexBox
                            {
                                Name  = "BoxBL",
                                Style = new()
                                {
                                    Color  = Color.White,
                                    Border = new(borderSize, 0, Color.White),
                                    Size   = new((Percentage)100, (Percentage)50),
                                }
                            },
                            new FlexBox
                            {
                                Name  = "BoxCL",
                                Style = new()
                                {
                                    Alignment = Alignment.Center,
                                    Color     = Color.White,
                                    Border    = new(borderSize, 0, Color.Yellow),
                                    Size      = new((Pixel)50),
                                }
                            }
                        ]
                    },
                    new FlexBox
                    {
                        Name = "StackR",
                        Style = new()
                        {
                            StackDirection = StackDirection.Vertical,
                            Border         = new(borderSize, 0, Color.Blue),
                            Size           = new((Percentage)100, (Percentage)100),
                        },
                        Children =
                        [
                            new FlexBox
                            {
                                Name  = "BoxAR",
                                Style = new()
                                {
                                    Color  = Color.White,
                                    Border = new(borderSize, 0, Color.Cyan),
                                    Size   = new((Pixel)50),
                                }
                            },
                            new FlexBox
                            {
                                Name  = "BoxBR",
                                Style = new()
                                {
                                    Color  = Color.White,
                                    Border = new(borderSize, 0, Color.White),
                                    Size   = new((Percentage)100),
                                }
                            },
                            new FlexBox
                            {
                                Name  = "BoxCR",
                                Style = new()
                                {
                                    Color  = Color.White,
                                    Border = new(borderSize, 0, Color.Yellow),
                                    Size   = new((Pixel)50),
                                }
                            }
                        ],
                    }
                ]
            }
        ];
    }
}
