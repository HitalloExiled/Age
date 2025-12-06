using Age.Core.Extensions;
using Age.Numerics;
using Age.Styling;
using static Age.Shaders.Geometry2DShader;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum AlignmentAxis
    {
        None       = 0,
        Baseline   = 1 << 0,
        Horizontal = 1 << 1,
        Vertical   = 1 << 2,
    }

    private static Point<float> GetAlignment(ItemsAlignment itemsAlignment, StackDirection direction, Alignment alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        alignmentAxis = AlignmentAxis.Horizontal | AlignmentAxis.Vertical;

        if (alignmentKind.HasFlags(Alignment.Left) || (direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start))))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Right) || (direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End))))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || (direction == StackDirection.Vertical && itemsAlignment == ItemsAlignment.Center))
        {
            x = 0;
        }
        else
        {
            alignmentAxis &= ~AlignmentAxis.Horizontal;
        }

        if (alignmentKind.HasFlags(Alignment.Top) || (direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start))))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Bottom) || (direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End))))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || (direction == StackDirection.Horizontal && itemsAlignment == ItemsAlignment.Center))
        {
            y = 0;
        }
        else
        {
            if (itemsAlignment == ItemsAlignment.Baseline || alignmentKind.HasFlags(Alignment.Baseline))
            {
                alignmentAxis |= AlignmentAxis.Baseline;
            }

            alignmentAxis &= ~AlignmentAxis.Vertical;
        }

        static float normalize(float value) =>
            (1 + value) / 2;

        return new(normalize(x), normalize(y));
    }
}
