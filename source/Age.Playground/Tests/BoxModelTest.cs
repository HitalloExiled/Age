using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class BoxModelTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        var boxModel = new FlexBox
        {
            Name = "BoxModel",
            Style = new()
            {
                Border    = new(borderSize, 0, Color.Red),
                Size      = new((Percentage)100),
                BoxSizing = BoxSizing.Border
            }
        };

        var stackL = new FlexBox
        {
            Name = "StackL",
            Style = new()
            {
                Stack  = StackKind.Vertical,
                Border = new(borderSize, 0, Color.Green),
                Size   = new((Percentage)50, (Percentage)100),
            }
        };

        var stackText = new FlexBox
        {
            Text  = "This elements should be stacked vertically...",
            Name  = "StackText",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                Color     = Color.White,
                Border    = new(10, 0, Color.Margenta),
            }
        };

        var boxAL = new FlexBox
        {
            Name  = "BoxAL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(borderSize, 0, Color.Cyan),
                Size   = new((Percentage)100, (Percentage)50),
            }
        };

        var boxBL = new FlexBox
        {
            Name  = "BoxBL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(borderSize, 0, Color.White),
                Size   = new((Percentage)100, (Percentage)50),
            }
        };

        var boxCL = new FlexBox
        {
            Name  = "BoxCL",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                Color     = Color.White,
                Border    = new(borderSize, 0, Color.Yellow),
                Size      = new((Pixel)50),
            }
        };

        var stackR = new FlexBox
        {
            Name = "StackR",
            Style = new()
            {
                Stack  = StackKind.Vertical,
                Border = new(borderSize, 0, Color.Blue),
                Size   = new((Percentage)100, (Percentage)100),
            }
        };

        var boxAR = new FlexBox
        {
            Name  = "BoxAR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(borderSize, 0, Color.Cyan),
                Size   = new((Pixel)50),
            }
        };

        var boxBR = new FlexBox
        {
            Name  = "BoxBR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(borderSize, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var boxCR = new FlexBox
        {
            Name  = "BoxCR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(borderSize, 0, Color.Yellow),
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
