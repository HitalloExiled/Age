using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class MarginTest
{
    public static void Setup(Canvas canvas)
    {
        const int BORDER_SIZE = 10;

        var margin = new Span
        {
            Name = "Margin",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = SizeUnit.Pixel(100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var a = new Span
        {
            Name = "Margin.A",
            Style = new()
            {
                // Margin = new(20),
                // Stack  = StackType.Vertical,
                Border = new(BORDER_SIZE, 0, Color.Green),
                Size   = new(Unit.Percentage(100)),
            }
        };

        canvas.AppendChild(margin);
            margin.AppendChild(a);
    }
}
