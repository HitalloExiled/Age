using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor;

public class Boxes : Element
{
    public override string NodeName => nameof(Boxes);

    public Boxes()
    {
        var boxH = new Span()
        {
            Name  = "BoxH",
            Style = new()
            {
                Stack = StackKind.Horizontal,
                Size  = new((Pixel)150),
            }
        };

        var boxV = new Span()
        {
            Name  = "BoxV",
            Style = new()
            {
                Stack = StackKind.Vertical,
                Size  = new((Pixel)150),
            }
        };

        this.AppendChild(boxH);
        this.AppendChild(boxV);

        const uint BLOCK_SIZE = 25;

        var ah = new Span()
        {
            Text  = "ah",
            Name  = "ah",
            Style = new()
            {
                // Alignment   = AlignmentType.Center | AlignmentType.Bottom,
                // BorderColor = Color.Red,
                Color       = Color.Red,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        var bh = new Span()
        {
            Text  = "bh",
            Name  = "bh",
            Style = new()
            {
                Alignment   = AlignmentType.Top,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new((Pixel)100),
            }
        };

        var ch = new Span()
        {
            Text  = "ch",
            Name  = "ch",
            Style = new()
            {
                // Alignment   = AlignmentType.Center | AlignmentType.Top,
                // BorderColor = Color.Blue,
                Color       = Color.Blue,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        var av = new Span()
        {
            Text  = "av",
            Name  = "av",
            Style = new()
            {
                // Alignment   = AlignmentType.Center,
                // BorderColor = Color.Red,
                Color       = Color.Red,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        var bv = new Span()
        {
            Text  = "bv",
            Name  = "bv",
            Style = new()
            {
                Alignment   = AlignmentType.Center/*  | AlignmentType.Top */,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        var cv = new Span()
        {
            Text  = "cv",
            Name  = "cv",
            Style = new()
            {
                // Alignment   = AlignmentType.Center,
                // BorderColor = Color.Blue,
                Color       = Color.Blue,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        boxH.AppendChild(ah);
        boxH.AppendChild(bh);
        boxH.AppendChild(ch);

        boxV.AppendChild(av);
        boxV.AppendChild(bv);
        boxV.AppendChild(cv);
    }
}
