using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class BoxModelTest
{
    public static void Setup(Canvas canvas)
    {
        const int BORDER_SIZE = 10;

        var boxModel = new Span
        {
            Name = "BoxModel",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = new((Percentage)100),
                BoxSizing = BoxSizing.Border
            }
        };

        var stackL = new Span
        {
            Name = "StackL",
            Style = new()
            {
                Stack  = StackType.Vertical,
                Border = new(BORDER_SIZE, 0, Color.Green),
                Size   = new((Percentage)50, (Percentage)100),
            }
        };

        var stackText = new Span
        {
            Text = "This elements should be stacked vertically...",
            Name  = "StackText",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Color     = Color.White,
                Border    = new(10, 0, Color.Margenta),
            }
        };

        var boxAL = new Span
        {
            Name  = "BoxAL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Cyan),
                Size   = new((Percentage)100, (Percentage)50),
            }
        };

        var boxBL = new Span
        {
            Name  = "BoxBL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100, (Percentage)50),
            }
        };

        var boxCL = new Span
        {
            Name  = "BoxCL",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Color     = Color.White,
                Border    = new(BORDER_SIZE, 0, Color.Yellow),
                Size      = new((Pixel)50),
            }
        };

        var stackR = new Span
        {
            Name = "StackR",
            Style = new()
            {
                Stack  = StackType.Vertical,
                Border = new(BORDER_SIZE, 0, Color.Blue),
                Size   = new((Percentage)100, (Percentage)100),
            }
        };

        var boxAR = new Span
        {
            Name  = "BoxAR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Cyan),
                Size   = new((Pixel)50),
            }
        };

        var boxBR = new Span
        {
            Name  = "BoxBR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var boxCR = new Span
        {
            Name  = "BoxCR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Yellow),
                Size   = new((Pixel)50),
            }
        };

        canvas.AppendChild(boxModel);
            boxModel.AppendChild(stackL);
                stackL.AppendChild(stackText);
                stackL.AppendChild(boxAL);
                stackL.AppendChild(boxBL);
                stackL.AppendChild(boxCL);

            boxModel.AppendChild(stackR);
                stackR.AppendChild(boxAR);
                stackR.AppendChild(boxBR);
                stackR.AppendChild(boxCR);
    }
}
