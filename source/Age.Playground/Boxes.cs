using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground;

public class Boxes : Element
{
    public override string NodeName => nameof(Boxes);

    public Boxes()
    {
        var boxH = new FlexBox()
        {
            Name  = "BoxH",
            Style = new()
            {
                StackDirection = StackDirection.Horizontal,
                Size           = new((Pixel)150),
            }
        };

        var boxV = new FlexBox()
        {
            Name  = "BoxV",
            Style = new()
            {
                StackDirection = StackDirection.Vertical,
                Size           = new((Pixel)150),
            }
        };

        this.AppendChild(boxH);
        this.AppendChild(boxV);

        const uint BLOCK_SIZE = 25;

        var ah = new FlexBox()
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

        var bh = new FlexBox()
        {
            Text  = "bh",
            Name  = "bh",
            Style = new()
            {
                Alignment   = Alignment.Top,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new((Pixel)100),
            }
        };

        var ch = new FlexBox()
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

        var av = new FlexBox()
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

        var bv = new FlexBox()
        {
            Text  = "bv",
            Name  = "bv",
            Style = new()
            {
                Alignment   = Alignment.Center/*  | AlignmentType.Top */,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new((Pixel)BLOCK_SIZE),
            }
        };

        var cv = new FlexBox()
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
