using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests.Styling;

public static class ClippingTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 10u;

        var n1_layer = new FlexBox()
        {
            Name  = "n1_layer",
            Style = new()
            {
                Border   = new(BORDER_SIZE, 100, Color.Red),
                Overflow = Overflow.Scroll,
                Size     = new(Unit.Px(50), Unit.Px(50)),
            }
        };

        n1_layer.Clicked += (in _) => n1_layer.Style.Overflow = n1_layer.Style.Overflow == Overflow.Scroll ? Overflow.None : Overflow.Scroll;

        var n2_a_layer = new FlexBox()
        {
            Name      = "n2_a_layer",
            InnerText = "Clipped\nContent",
            Style     = new()
            {
                Border    = new(BORDER_SIZE, 60, Color.Green),
                Color     = Color.White,
                FontSize  = 24,
                // Transform = Transform2D.CreateTranslated(25, -40),
                // Overflow  = OverflowKind.Clipping,
                // Size      = new(Unit.Px(100), Unit.Px(100)),
            }
        };

        n2_a_layer.Clicked += (in _) =>
        {
            n2_a_layer.Style.Overflow = n2_a_layer.Style.Overflow == Overflow.Clipping ? Overflow.None : Overflow.Clipping;
        };

        //n2_a_layer.Clicked += (in MouseEvent _) =>
        //    n2_a_layer.Detach();

        var n3_no_layer = new FlexBox()
        {
            Name      = "n3_no_layer",
            InnerText = "X\nY\nZ",
            Style     = new()
            {
                Color                = Color.White,
                FontSize             = 36,
                ContentJustification = ContentJustification.SpaceAround,
                Border               = new(BORDER_SIZE, 60, Color.Blue),
                //Overflow           = OverflowKind.Clipping,
                //BackgroundColor    = Color.Margenta,
                Transforms           = [TransformOp.Translate(new(Unit.Px(25), Unit.Px(60)))],
                Size                 = new(Unit.Px(100), Unit.Px(100)),
            }
        };

        var n4_layer = new FlexBox()
        {
            Name  = "n4_layer",
            Style = new()
            {
                Border      = new(1, 0, Color.Blue),
                // Overflow = OverflowKind.Clipping,
                Transforms  = [TransformOp.Translate(Unit.Px(-20), Unit.Px(20))],
                Size        = new(Unit.Px(200), Unit.Px(100)),
            }
        };

        var n5_no_layer = new FlexBox()
        {
            Name  = "n5_no_layer",
            Style = new()
            {
                Border = new(BORDER_SIZE, 50, Color.Green),
                Size   = new(Unit.Px(200), Unit.Px(100)),
            }
        };

        var n2_b_non_layer = new FlexBox()
        {
            Name  = "n2_b_non_layer",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Green),
                Size   = new(Unit.Px(200), Unit.Px(100)),
            }
        };

        canvas.AppendChild(n1_layer);
           n1_layer.AppendChild(n2_a_layer);
            //    n2_a_layer.AppendChild(n3_no_layer);
                //    n3_no_layer.AppendChild(n4_layer);
                //        n4_layer.AppendChild(n5_no_layer);
            //canvas.AppendChild(n2_b_non_layer);

        //                 n4_layer.AppendChild(n5_no_layer);
        //             n3_no_layer.AppendChild(n4_layer);
        //         n2_a_layer.AppendChild(n3_no_layer);
        //     n1_layer.AppendChild(n2_a_layer);
        //     n1_layer.AppendChild(n2_b_non_layer);
        // canvas.AppendChild(n1_layer);
    }
}
