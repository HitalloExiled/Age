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
                Size           = new(Unit.Px(150)),
            }
        };

        var boxV = new FlexBox()
        {
            Name  = "BoxV",
            Style = new()
            {
                StackDirection = StackDirection.Vertical,
                Size           = new(Unit.Px(150)),
            }
        };

        this.AppendChild(boxH);
        this.AppendChild(boxV);

        const int BLOCK_SIZE = 25;

        var ah = new FlexBox()
        {
            InnerText = "ah",
            Name      = "ah",
            Style     = new()
            {
                // Alignment   = AlignmentType.Center | AlignmentType.Bottom,
                // BorderColor = Color.Red,
                Color       = Color.Red,
                Size        = new(Unit.Px(BLOCK_SIZE)),
            }
        };

        var bh = new FlexBox()
        {
            InnerText = "bh",
            Name      = "bh",
            Style     = new()
            {
                Alignment   = Alignment.Top,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new(Unit.Px(100)),
            }
        };

        var ch = new FlexBox()
        {
            InnerText = "ch",
            Name      = "ch",
            Style     = new()
            {
                // Alignment   = AlignmentType.Center | AlignmentType.Top,
                // BorderColor = Color.Blue,
                Color       = Color.Blue,
                Size        = new(Unit.Px(BLOCK_SIZE)),
            }
        };

        var av = new FlexBox()
        {
            InnerText = "av",
            Name      = "av",
            Style     = new()
            {
                // Alignment   = AlignmentType.Center,
                // BorderColor = Color.Red,
                Color       = Color.Red,
                Size        = new(Unit.Px(BLOCK_SIZE)),
            }
        };

        var bv = new FlexBox()
        {
            InnerText = "bv",
            Name      = "bv",
            Style     = new()
            {
                Alignment   = Alignment.Center/*  | AlignmentType.Top */,
                // BorderColor = Color.Green,
                Color       = Color.Green,
                Size        = new(Unit.Px(BLOCK_SIZE)),
            }
        };

        var cv = new FlexBox()
        {
            InnerText = "cv",
            Name      = "cv",
            Style     = new()
            {
                // Alignment   = AlignmentType.Center,
                // BorderColor = Color.Blue,
                Color       = Color.Blue,
                Size        = new(Unit.Px(BLOCK_SIZE)),
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
