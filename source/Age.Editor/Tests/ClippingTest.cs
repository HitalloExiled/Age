using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class ClippingTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        var n1_layer = new FlexBox()
        {
            Name  = "n1_layer",
            Style = new()
            {
                Border   = new(borderSize, 50, Color.Green),
                Overflow = OverflowKind.Clipping,
                Size     = new((Pixel)100, (Pixel)200),
            }
        };

        var n2_a_layer = new FlexBox()
        {
            Name  = "n2_a_layer",
            Style = new()
            {
                Border   = new(borderSize, 50, Color.Blue),
                Overflow = OverflowKind.Clipping,
                Size     = new((Pixel)200, (Pixel)100),
            }
        };

        var n3_no_layer = new FlexBox()
        {
            Name  = "n3_no_layer",
            Style = new()
            {
                Border = new(borderSize, 50, Color.Green),
                Size   = new((Pixel)200, (Pixel)100),
            }
        };

        var n4_layer = new FlexBox()
        {
            Name  = "n4_layer",
            Style = new()
            {
                Border   = new(borderSize, 0, Color.Blue),
                Overflow = OverflowKind.Clipping,
                Size     = new((Pixel)200, (Pixel)100),
            }
        };

        var n5_no_layer = new FlexBox()
        {
            Name  = "n5_no_layer",
            Style = new()
            {
                Border = new(borderSize, 50, Color.Green),
                Size   = new((Pixel)200, (Pixel)100),
            }
        };

        var n2_b_non_layer = new FlexBox()
        {
            Name  = "n2_b_non_layer",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Size   = new((Pixel)200, (Pixel)100),
            }
        };

        canvas.AppendChild(n1_layer);
            n1_layer.AppendChild(n2_a_layer);
                //n2_a_layer.AppendChild(n3_no_layer);
                //    n3_no_layer.AppendChild(n4_layer);
                //        n4_layer.AppendChild(n5_no_layer);

            n1_layer.AppendChild(n2_b_non_layer);
    }
}
