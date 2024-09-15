using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class DivTest
{
    public static void Setup(Canvas canvas, in TestContext context)
    {
        var vertical = new Div()
        {
            Name  = "vertical",
            Style = new()
            {
                Border  = new(context.BorderSize, 0, Color.Margenta),
                Color   = Color.White,
                Size    = new((Pixel)(20 * context.Scale)),
            }
        };

        var hello = new Span()
        {
            Name  = "hello",
            Text  = "Hello",
            Style = new()
            {
                Border   = new(context.BorderSize, 0, Color.Red),
                Color    = Color.Red,
                FontSize = 24,
                Margin   = new((Pixel)2, null),
            }
        };

        var world = new Span()
        {
            Name  = "world",
            Text  = "World",
            Style = new()
            {
                Border   = new(context.BorderSize, 0, Color.Green),
                Color    = Color.Green,
                FontSize = 12,
                Margin   = new((Pixel)2, null),
            }
        };

        var a = new Div()
        {
            Name  = "a",
            Text  = "!!!",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                Border    = new(context.BorderSize, 0, Color.Blue),
                Color     = Color.Blue,
                FontSize  = 12,
                Size      = new((Pixel)20, (Percentage)50),
                MaxSize   = new(null,      (Pixel)200),
                //BoxSizing = BoxSizing.Border,
                //Margin = new((Pixel)10),
            }
        };

        canvas.AppendChild(vertical);
            //vertical.AppendChild(hello);
            //vertical.AppendChild(world);
            vertical.AppendChild(a);
    }
}
