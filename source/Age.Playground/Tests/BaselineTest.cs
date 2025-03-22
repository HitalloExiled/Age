using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class BaselineTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;

        var box = new FlexBox()
        {
            Name  = "box",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Cyan),
                Size   = new((Pixel)100),
            }
        };

        var vertical_text = new FlexBox()
        {
            Name  = "vertical_text",
            Text  = "Vertical\nText",
            Style = new()
            {
                Alignment  = AlignmentKind.Baseline,
                Border     = new(borderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 12,
            }
        };

        var horizontal_a_container = new FlexBox()
        {
            Name  = "horizontal_container",
            Text  = "Horizontal",
            Style = new()
            {
                Padding        = new((Pixel)20),
                Border         = new(borderSize, 0, Color.Cyan),
                Color          = Color.White,
                FontFamily     = "Impact",
                FontSize       = 24,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
            }
        };

        var horizontal_a_child1 = new FlexBox
        {
            Name = "X",
            Text = "X",
            Style = new()
            {
                Border     = new(borderSize * 2, 0, Color.Red),
                Color      = Color.Red,
                FontFamily = "Helvetica Neue",
                FontSize   = 48,
                Hidden     = false,
                Margin     = new(null, (Pixel)10),
            }
        };

        var horizontal_a_child2 = new FlexBox
        {
            Name = "Y",
            Text = "Y",
            Style = new()
            {
                Border     = new(borderSize / 2, 0, Color.Green),
                Color      = Color.Green,
                FontFamily = "Lucida Console",
                FontSize   = 24,
            }
        };

        var horizontal_a_child3 = new FlexBox
        {
            Name = "Z",
            Text = "Z",
            Style = new()
            {
                Border     = new(borderSize, 0,     Color.Blue),
                Color      = Color.Blue,
                FontFamily = "Verdana",
                FontSize   = 48,
            }
        };

        var horizontal_a_child4 = new FlexBox
        {
            Name = "Hello",
            Text = "Hello",
            Style = new()
            {
                Alignment = AlignmentKind.Start,
                Border    = new(borderSize, 0, Color.Margenta),
                Color     = Color.White,
                Margin    = new((Pixel)5)
            }
        };

        var horizontal_a_child5 = new FlexBox
        {
            Name  = "World",
            Text  = "World!!!",
            Style = new()
            {
                Alignment = AlignmentKind.End,
                Border    = new(borderSize, 0, Color.Margenta),
                Color     = Color.White,
                Margin    = new((Pixel)5)
            }
        };

        var horizontal_b_container = new FlexBox()
        {
            Name  = "horizontal_container",
            Text  = "Horizontal",
            Style = new()
            {
                Alignment      = AlignmentKind.Baseline,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
                Border         = new(borderSize, 0, Color.Margenta),
                Color          = Color.White,
                FontFamily     = "Impact",
                FontSize       = 12,
            }
        };

        var horizontal_b_child1 = new FlexBox
        {
            Name = "X",
            Text = "X",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Red),
                Color      = Color.Red,
                FontFamily = "Helvetica Neue",
                FontSize   = 24,
            }
        };

        var horizontal_b_child2 = new FlexBox
        {
            Name = "Y",
            Text = "Y",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Green),
                Color      = Color.Green,
                FontFamily = "Lucida Console",
                FontSize   = 12,
            }
        };

        var horizontal_b_child3 = new FlexBox
        {
            Name = "Z",
            Text = "Z",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Blue),
                Color      = Color.Blue,
                FontFamily = "Verdana",
                FontSize   = 24,
            }
        };

        var horizontal_c_container = new FlexBox
        {
            Name  = "horizontal_c_container",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Horizontal,
            }
        };

        var horizontal_c_child1 = new FlexBox
        {
            Name  = "horizontal_c_child1",
            Style = new()
            {
                Alignment = AlignmentKind.Left,
                Border    = new(borderSize, 0, Color.Cyan),
                Margin    = new((Pixel)10),
                Size      = new((Pixel)100),
            }
        };

        var horizontal_c_child2 = new FlexBox
        {
            Name  = "horizontal_c_child2",
            Style = new()
            {
                Alignment = AlignmentKind.Right,
                Border    = new(borderSize, 0, Color.Cyan),
                Margin    = new((Pixel)10),
                Size      = new((Pixel)100),
            }
        };

        var vertical_a_container = new FlexBox
        {
            Name  = "vertical_a_container",
            // Text  = "Vertical",
            Style = new()
            {
                Alignment  = AlignmentKind.Baseline,
                Border     = new(borderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_a_child1 = new FlexBox
        {
            Name = "X",
            Text = "X",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Red),
                Color      = Color.Red,
                FontFamily = "Helvetica Neue",
                FontSize   = 48,
            }
        };

        var vertical_a_child2 = new FlexBox
        {
            Name = "Y",
            Text = "Y",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Green),
                Color      = Color.Green,
                FontFamily = "Lucida Console",
                FontSize   = 24,
            }
        };

        var vertical_a_child3 = new FlexBox
        {
            Name = "Z",
            Text = "Z",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Blue),
                Color      = Color.Blue,
                FontFamily = "Verdana",
                FontSize   = 48,
            }
        };

        var vertical_a_child4 = new FlexBox
        {
            Text = "Hello",
            Style = new()
            {
                Alignment = AlignmentKind.Top,
                Border    = new(borderSize, 0, Color.Margenta),
                Color     = Color.White,
                Margin    = new((Pixel)10),
            }
        };

        var vertical_a_child5 = new FlexBox
        {
            Text = "World!!!",
            Style = new()
            {
                Alignment = AlignmentKind.Bottom,
                Border    = new(borderSize, 0, Color.Margenta),
                Color     = Color.White,
                Margin    = new((Pixel)10),
            }
        };

        var vertical_b_container = new FlexBox
        {
            Name  = "vertical_b_container",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_b_child1 = new FlexBox
        {
            Name = "vertical_b_child1",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)200),
                Border = new(borderSize, 0, Color.Red),
            }
        };

        var vertical_b_child2 = new FlexBox
        {
            Name = "vertical_b_child2",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)100),
                Border = new(borderSize, 0, Color.Green),
            }
        };

        var vertical_b_child3 = new FlexBox
        {
            Name = "vertical_b_child3",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)50),
                Border = new(borderSize, 0, Color.Blue),
            }
        };

        var vertical_c_container = new FlexBox
        {
            Name  = "vertical_c_container",
            Style = new()
            {
                Alignment  = AlignmentKind.Baseline,
                Border     = new(borderSize, 0, Color.Cyan),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_c_child1 = new FlexBox { Name = "Vertical", Text = "Vertical", Style = new() { Border = new(borderSize, 0, Color.Red), Margin = new(null, (Pixel)0), FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var vertical_c_child2 = new FlexBox { Name = "X", Text = "X", Style = new() { Border = new(borderSize, 0, Color.Red),   FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var vertical_c_child3 = new FlexBox { Name = "Y", Text = "Y", Style = new() { Border = new(borderSize, 0, Color.Green), FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        var vertical_c_child4 = new FlexBox { Name = "Z", Text = "Z", Style = new() { Border = new(borderSize, 0, Color.Blue),  FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } };
        var vertical_c_child5 = new FlexBox { Text = "Hello",         Style = new() { Border = new(borderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentKind.Start, Margin = new((Pixel)5) } };
        var vertical_c_child6 = new FlexBox { Text = "World!!!",      Style = new() { Border = new(borderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentKind.End,   Margin = new((Pixel)5) } };

        var vertical_d_container = new FlexBox
        {
            Name  = "vertical_c_container",
            Style = new()
            {
                Border     = new(borderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
                Alignment      = AlignmentKind.Baseline,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
            }
        };

        var vertical_d_child1 = new FlexBox
        {
            Name  = "vertical_d_child1",
            Style = new()
            {
                Size      = new((Pixel)100),
                Margin    = new((Pixel)10),
                Border    = new(borderSize, 0, Color.Cyan),
                Alignment = AlignmentKind.Top,
            }
        };

        var vertical_d_child2 = new FlexBox
        {
            Name  = "vertical_d_child2",
            Style = new()
            {
                Size      = new((Pixel)100),
                Margin    = new((Pixel)10),
                Border    = new(borderSize, 0, Color.Cyan),
                Alignment = AlignmentKind.Bottom,
            }
        };

        canvas.AppendChild(new FrameStatus());
        canvas.AppendChild(box);
        canvas.AppendChild(vertical_text);

        canvas.AppendChild(horizontal_a_container);
            horizontal_a_container.AppendChild(horizontal_a_child1);
            horizontal_a_container.AppendChild(horizontal_a_child2);
            horizontal_a_container.AppendChild(horizontal_a_child3);
            horizontal_a_container.AppendChild(horizontal_a_child4);
            horizontal_a_container.AppendChild(horizontal_a_child5);

        canvas.AppendChild(horizontal_b_container);
            horizontal_b_container.AppendChild(horizontal_b_child1);
            horizontal_b_container.AppendChild(horizontal_b_child2);
            horizontal_b_container.AppendChild(horizontal_b_child3);

        canvas.AppendChild(horizontal_c_container);
            horizontal_c_container.AppendChild(horizontal_c_child1);
            horizontal_c_container.AppendChild(horizontal_c_child2);

        canvas.AppendChild(vertical_a_container);
            vertical_a_container.AppendChild(vertical_a_child1);
            vertical_a_container.AppendChild(vertical_a_child2);
            vertical_a_container.AppendChild(vertical_a_child3);
            vertical_a_container.AppendChild(vertical_a_child4);
            vertical_a_container.AppendChild(vertical_a_child5);

        canvas.AppendChild(vertical_b_container);
            vertical_b_container.AppendChild(vertical_b_child1);
            vertical_b_container.AppendChild(vertical_b_child2);
            vertical_b_container.AppendChild(vertical_b_child3);

        canvas.AppendChild(vertical_c_container);
            vertical_c_container.AppendChild(vertical_c_child1);
            vertical_c_container.AppendChild(vertical_c_child2);
            vertical_c_container.AppendChild(vertical_c_child3);
            vertical_c_container.AppendChild(vertical_c_child4);
            vertical_c_container.AppendChild(vertical_c_child5);
            vertical_c_container.AppendChild(vertical_c_child6);

        canvas.AppendChild(vertical_d_container);
            vertical_d_container.AppendChild(vertical_d_child1);
            vertical_d_container.AppendChild(vertical_d_child2);
    }
}
