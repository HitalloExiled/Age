using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class MarginTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        var box_noMargin = new FlexBox
        {
            Name  = "noMargin",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Margenta),
            }
        };

        var noMargin = new FlexBox
        {
            Name = "noMargin",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Margenta * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var box_pc50_px10 = new FlexBox
        {
            Name  = "box_pc50_px10",
            Style = new()
            {
                Border    = new(borderSize, 0, Color.Red),
            }
        };

        var margin_pc50_px10 = new FlexBox
        {
            Name  = "margin_pc50_px10",
            Style = new()
            {
                Margin = new((Pixel)10),
                Border = new(borderSize, 0, Color.Red * 0.8f),
                Size   = new((Percentage)50),
            }
        };

        var box_px100_px10 = new FlexBox
        {
            Name  = "box_px100_px10",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
            }
        };

        var margin_px100_px10 = new FlexBox
        {
            Name  = "margin_px100_px10",
            Style = new()
            {
                Margin = new((Pixel)10),
                Border = new(borderSize, 0, Color.Green * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var box_px100_pc10 = new FlexBox
        {
            Name  = "box_px100_pc10",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
            }
        };

        var margin_px100_pc10 = new FlexBox
        {
            Name  = "margin_px100_pc10",
            Style = new()
            {
                Margin = new((Percentage)10),
                Border = new(borderSize, 0, Color.Blue * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var box_pc100_pc10 = new FlexBox
        {
            Name  = "box_pc100_pc10",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Cyan),
            }
        };

        var margin_pc100_pc10 = new FlexBox
        {
            Name  = "margin_pc100_pc10",
            Style = new()
            {
                Margin = new((Percentage)10),
                Border = new(borderSize, 0, Color.Cyan * 0.8f),
                Size   = new((Percentage)100),
            }
        };

        canvas.AppendChild(box_noMargin);
            box_noMargin.AppendChild(noMargin);

        canvas.AppendChild(box_pc50_px10);
            box_pc50_px10.AppendChild(margin_pc50_px10);

        canvas.AppendChild(box_px100_px10);
            box_px100_px10.AppendChild(margin_px100_px10);

        canvas.AppendChild(box_px100_pc10);
            box_px100_pc10.AppendChild(margin_px100_pc10);

        canvas.AppendChild(box_pc100_pc10);
            box_pc100_pc10.AppendChild(margin_pc100_pc10);
    }
}
