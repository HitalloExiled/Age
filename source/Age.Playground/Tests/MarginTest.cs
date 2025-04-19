using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class MarginTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        canvas.Children =
        [
            new FlexBox
            {
                Name  = "box_noMargin",
                Style = new()
                {
                    Border = new(borderSize, 0, Color.Margenta),
                },
                Children =
                [
                    new FlexBox
                    {
                        Name = "noMargin",
                        Style = new()
                        {
                            Border = new(borderSize, 0, Color.Margenta * 0.8f),
                            Size   = new((Pixel)100),
                        }
                    }
                ]
            },
            new FlexBox
            {
                Name  = "box_pc50_px10",
                Style = new()
                {
                    Border    = new(borderSize, 0, Color.Red),
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "margin_pc50_px10",
                        Style = new()
                        {
                            Margin = new((Pixel)10),
                            Border = new(borderSize, 0, Color.Red * 0.8f),
                            Size   = new((Percentage)50),
                        }
                    }
                ]
            },
            new FlexBox
            {
                Name  = "box_px100_px10",
                Style = new()
                {
                    Border = new(borderSize, 0, Color.Green),
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "margin_px100_px10",
                        Style = new()
                        {
                            Margin = new((Pixel)10),
                            Border = new(borderSize, 0, Color.Green * 0.8f),
                            Size   = new((Pixel)100),
                        }
                    }
                ]
            },
            new FlexBox
            {
                Name  = "box_px100_pc10",
                Style = new()
                {
                    Border = new(borderSize, 0, Color.Blue),
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "margin_px100_pc10",
                        Style = new()
                        {
                            Margin = new((Percentage)10),
                            Border = new(borderSize, 0, Color.Blue * 0.8f),
                            Size   = new((Pixel)100),
                        }
                    }
                ]
            },
            new FlexBox
            {
                Name  = "box_pc100_pc10",
                Style = new()
                {
                    Border = new(borderSize, 0, Color.Cyan),
                },
                Children =
                [
                    new FlexBox
                    {
                        Name  = "margin_pc100_pc10",
                        Style = new()
                        {
                            Margin = new((Percentage)10),
                            Border = new(borderSize, 0, Color.Cyan * 0.8f),
                            Size   = new((Percentage)100),
                        }
                    }
                ]
            },
        ];
    }
}
