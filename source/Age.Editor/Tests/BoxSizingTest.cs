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
            Name  = "BSC",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Cyan),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bscA100 = new Span
        {
            Name  = "BSC-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bscw100 = new Span
        {
            Name  = "BSCw100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Cyan),
                Size      = new((Pixel)100, null),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bscw100A100 = new Span
        {
            Name  = "BSCw100-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bsch100 = new Span
        {
            Name  = "BSCh100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Cyan),
                Size      = new(null, (Pixel)100),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bsch100A100 = new Span
        {
            Name  = "BSCh100-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bsb = new Span
        {
            Name  = "BSB",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Margenta),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbA100 = new Span
        {
            Name  = "BSB-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bsbw100 = new Span
        {
            Name  = "BSBw100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Margenta),
                Size      = new((Pixel)100, null),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbw100A100 = new Span
        {
            Name  = "BSBw100-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bsbh100 = new Span
        {
            Name  = "BSCh100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Margenta),
                Size      = new(null, (Pixel)100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbh100A100 = new Span
        {
            Name  = "BSBh100-A100",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new((Percentage)100),
            }
        };

        var bsc100 = new Span
        {
            Name  = "BSC100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = new((Pixel)100),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bsc100A = new Span
        {
            Name  = "BSC100-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.9f),
                Size   = new((Percentage)50),
            }
        };

        var bsc100B = new Span
        {
            Name  = "BSC100-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.8f),
                Size   = new((Percentage)50),
            }
        };

        var bsb100 = new Span
        {
            Name  = "BSB100",
            Style = new()
            {
                Border    = new(BORDER_SIZE, 0, Color.Green),
                Size      = new((Pixel)100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsb100A = new Span
        {
            Name  = "BSB100-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.9f),
                Size   = new((Percentage)50),
            }
        };

        var bsb100B = new Span
        {
            Name  = "BSB100-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.8f),
                Size   = new((Percentage)50),
            }
        };

        var bsc100v = new Span
        {
            Name  = "BSC100v",
            Style = new()
            {
                Stack     = StackType.Vertical,
                Border    = new(BORDER_SIZE, 0, Color.Red),
                Size      = new((Pixel)100),
                BoxSizing = BoxSizing.Content,
            }
        };

        var bscv100A = new Span
        {
            Name  = "BSC100v-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.9f),
                Size   = new((Percentage)50),
            }
        };

        var bscv100B = new Span
        {
            Name  = "BSC100v-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.8f),
                Size   = new((Percentage)50),
            }
        };

        var bsb100v = new Span
        {
            Name  = "BSB100v",
            Style = new()
            {
                Stack     = StackType.Vertical,
                Border    = new(BORDER_SIZE, 0, Color.Green),
                Size      = new((Pixel)100),
                BoxSizing = BoxSizing.Border,
            }
        };

        var bsbv100A = new Span
        {
            Name  = "BSB100v-A",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.9f),
                Size   = new((Percentage)50),
            }
        };

        var bsbv100B = new Span
        {
            Name  = "BSB100v-B",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green * 0.8f),
                Size   = new((Percentage)50),
            }
        };

        canvas.AppendChild(bsc);
            bsc.AppendChild(bscA100);

        canvas.AppendChild(bscw100);
            bscw100.AppendChild(bscw100A100);

        canvas.AppendChild(bsch100);
            bsch100.AppendChild(bsch100A100);

        canvas.AppendChild(bsb);
            bsb.AppendChild(bsbA100);

        canvas.AppendChild(bsbw100);
            bsbw100.AppendChild(bsbw100A100);

        canvas.AppendChild(bsbh100);
            bsbh100.AppendChild(bsbh100A100);

        canvas.AppendChild(bsc100);
            bsc100.AppendChild(bsc100A);
            bsc100.AppendChild(bsc100B);

        canvas.AppendChild(bsb100);
            bsb100.AppendChild(bsb100A);
            bsb100.AppendChild(bsb100B);

        canvas.AppendChild(bsc100v);
            bsc100v.AppendChild(bscv100A);
            bsc100v.AppendChild(bscv100B);

        canvas.AppendChild(bsb100v);
            bsb100v.AppendChild(bsbv100A);
            bsb100v.AppendChild(bsbv100B);
    }
}