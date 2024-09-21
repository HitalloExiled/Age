using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class AlignmentTest
{
    public static void Setup(Canvas canvas, in TestContext testContext)
    {

        var horizontal_a_container = new FlexBox()
        {
            Name  = "horizontal_a_container",
            Text  = "Horizonta A",
            Style = new()
            {
                Border         = new(testContext.BorderSize, 0, Color.Margenta),
                Color          = Color.White,
                FontSize       = 32,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
            }
        };

        var horizontal_a_left = new FlexBox()
        {
            Name  = "horizontal_a_left",
            Text  = "Left",
            Style = new()
            {
                Alignment = AlignmentKind.Left,
                Border    = new(testContext.BorderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var horizontal_a_right = new FlexBox()
        {
            Name  = "horizontal_a_right",
            Text  = "Right",
            Style = new()
            {
                Alignment = AlignmentKind.Right,
                Border    = new(testContext.BorderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var horizontal_b_container = new FlexBox()
        {
            Name  = "horizontal_b_container",
            Text  = "Horizonta B",
            Style = new()
            {
                Border   = new(testContext.BorderSize, 0, Color.Margenta),
                FontSize = 32,
                Color    = Color.White,
            }
        };

        var horizontal_b_top = new FlexBox()
        {
            Name  = "horizontal_b_top",
            Text  = "Top",
            Style = new()
            {
                Alignment = AlignmentKind.Top,
                Border    = new(testContext.BorderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var horizontal_b_bottom = new FlexBox()
        {
            Name  = "horizontal_b_bottom",
            Text  = "Down",
            Style = new()
            {
                Alignment = AlignmentKind.Bottom,
                Border    = new(testContext.BorderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        canvas.AppendChild(horizontal_a_container);
            horizontal_a_container.AppendChild(horizontal_a_left);
            horizontal_a_container.AppendChild(horizontal_a_right);

        canvas.AppendChild(horizontal_b_container);
            horizontal_b_container.AppendChild(horizontal_b_top);
            horizontal_b_container.AppendChild(horizontal_b_bottom);
    }
}
