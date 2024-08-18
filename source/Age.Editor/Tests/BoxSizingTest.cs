using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class BoxSizingTest
{
    public static void Setup(Canvas canvas)
    {
        const int BORDER_SIZE = 10;

        var bsc = new Span
        {
            Name = "BSC",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = SizeUnit.Pixel(100),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bscA = new Span
        {
            Name = "BSC-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.9f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bscB = new Span
        {
            Name = "BSC-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.8f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bsb = new Span
        {
            Name  = "BSB",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Green),
                Size      = SizeUnit.Pixel(100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbA = new Span
        {
            Name = "BSB-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.9f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bsbB = new Span
        {
            Name = "BSB-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.8f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bscv = new Span
        {
            Name = "BSCv",
            Style = new()
            {
                Stack     = StackType.Vertical,
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = SizeUnit.Pixel(100),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bscvA = new Span
        {
            Name = "BSCv-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.9f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bscvB = new Span
        {
            Name = "BSCv-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.8f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bsbv = new Span
        {
            Name  = "BSBv",
            Style = new()
            {
                Stack     = StackType.Vertical,
                Border    = new(BORDER_SIZE, 0, Color.Green),
                Size      = SizeUnit.Pixel(100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbvA = new Span
        {
            Name = "BSBv-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.9f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        var bsbvB = new Span
        {
            Name = "BSBv-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.8f),
                Size   = new(Unit.Percentage(50)),
            }
        };

        canvas.AppendChild(bsc);
            bsc.AppendChild(bscA);
            bsc.AppendChild(bscB);

        canvas.AppendChild(bsb);
            bsb.AppendChild(bsbA);
            bsb.AppendChild(bsbB);

        canvas.AppendChild(bscv);
            bscv.AppendChild(bscvA);
            bscv.AppendChild(bscvB);

        canvas.AppendChild(bsbv);
            bsbv.AppendChild(bsbvA);
            bsbv.AppendChild(bsbvB);
    }
}
