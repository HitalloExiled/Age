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
                Border   = new Border
                {
                    Top    = new(10, Color.Blue),
                    Right  = new(10, Color.Blue),
                    Bottom = new(10, Color.Blue),
                    Left   = new(10, Color.Blue),
                    Radius =
                    {
                        LeftTop     = 0,
                        TopRight    = 40,
                        RightBottom = 20,
                        BottomLeft  = 0,
                    }
                },
                Overflow = OverflowKind.Clipping,
                Size     = new((Pixel)100, (Pixel)200),
            }
        };

        var n2_a_layer = new FlexBox()
        {
            Name  = "n2_a_layer",
            Style = new()
            {
                Border    = new(borderSize, 30, new(0, 0, 1, 0.5f)),
                Transform = Transform2D.CreateRotated(Angle.Radians(0)),
                Overflow  = OverflowKind.Clipping,
                Size      = new((Pixel)200, (Pixel)100),
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
                Border   = new(1, 0, Color.Blue),
                Overflow = OverflowKind.Clipping,
                Transform = Transform2D.CreateRotated(Angle.Radians(-15)),
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

        //canvas.AppendChild(n1_layer);
        //    n1_layer.AppendChild(n2_a_layer);
        //        n2_a_layer.AppendChild(n3_no_layer);
        //            n3_no_layer.AppendChild(n4_layer);
        //                n4_layer.AppendChild(n5_no_layer);

        //n1_layer.AppendChild(n2_b_non_layer);


                        n4_layer.AppendChild(n5_no_layer);
                    n3_no_layer.AppendChild(n4_layer);
                n2_a_layer.AppendChild(n3_no_layer);
            n1_layer.AppendChild(n2_a_layer);
            n1_layer.AppendChild(n2_b_non_layer);
        canvas.AppendChild(n1_layer);
    }
}
