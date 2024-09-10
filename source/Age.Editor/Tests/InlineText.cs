using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class InlineText
{
    public static uint BorderSize { get; set; } = 1;

    public static void Setup(Canvas canvas)
    {
        var horizontal_a_container = new Span()
        {
            Name  = "horizontal_container",
            Text  = "Horizontal",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
            }
        };

        var horizontal_a_child1 = new Span { Name = "X",     Text = "X",        Style = new() { Border = new(BorderSize * 2, 0,     Color.Red),      FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red, Margin = new(null, (Pixel)10) } };
        var horizontal_a_child2 = new Span { Name = "Y",     Text = "Y",        Style = new() { Border = new(BorderSize / 2, 0, Color.Green),    FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        var horizontal_a_child3 = new Span { Name = "Z",     Text = "Z",        Style = new() { Border = new(BorderSize, 0,     Color.Blue),     FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } };
        var horizontal_a_child4 = new Span { Name = "Hello", Text = "Hello",    Style = new() { Border = new(BorderSize, 0,     Color.Margenta), Color = Color.White, Alignment = AlignmentType.Top,    Margin = new((Pixel)4) } };
        var horizontal_a_child5 = new Span { Name = "World", Text = "World!!!", Style = new() { Border = new(BorderSize, 0,     Color.Margenta), Color = Color.White, Alignment = AlignmentType.Bottom, Margin = new((Pixel)4) } };

        var horizontal_b_container = new Span()
        {
            Name  = "horizontal_container",
            Text  = "Horizontal",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 12,
            }
        };

        var horizontal_b_child1 = new Span { Name = "X", Text = "X", Style = new() { Border = new(BorderSize, 0, Color.Red),   FontSize = 24, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var horizontal_b_child2 = new Span { Name = "Y", Text = "Y", Style = new() { Border = new(BorderSize, 0, Color.Green), FontSize = 12, FontFamily = "Lucida Console", Color = Color.Green } };
        var horizontal_b_child3 = new Span { Name = "Z", Text = "Z", Style = new() { Border = new(BorderSize, 0, Color.Blue),  FontSize = 24, FontFamily = "Verdana",        Color = Color.Blue } };

        var vertical_a_container = new Span
        {
            Name  = "vertical_a_container",
            // Text  = "Vertical",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_a_child1 = new Span { Name = "X", Text = "X", Style = new() { Border = new(BorderSize, 0, Color.Red),   FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var vertical_a_child2 = new Span { Name = "Y", Text = "Y", Style = new() { Border = new(BorderSize, 0, Color.Green), FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        var vertical_a_child3 = new Span { Name = "Z", Text = "Z", Style = new() { Border = new(BorderSize, 0, Color.Blue),  FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } };
        var vertical_a_child4 = new Span { Text = "Hello",         Style = new() { Border = new(BorderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentType.Top,    Margin = new((Pixel)10) } };
        var vertical_a_child5 = new Span { Text = "World!!!",      Style = new() { Border = new(BorderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentType.Bottom, Margin = new((Pixel)10) } };

        var vertical_b_container = new Span
        {
            Name  = "vertical_b_container",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_b_child1 = new Span
        {
            Name = "vertical_b_child1",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)200),
                Border = new(BorderSize, 0, Color.Red),
            }
        };

        var vertical_b_child2 = new Span
        {
            Name = "vertical_b_child2",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)100),
                Border = new(BorderSize, 0, Color.Green),
            }
        };

        var vertical_b_child3 = new Span
        {
            Name = "vertical_b_child3",
            Style = new()
            {
                Size   = new((Pixel)10, (Pixel)50),
                Border = new(BorderSize, 0, Color.Blue),
            }
        };

        var vertical_c_container = new Span
        {
            Name  = "vertical_c_container",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_c_child1 = new Span { Name = "Vertical", Text = "Vertical", Style = new() { Border = new(BorderSize, 0, Color.Red), Margin = new(null, (Pixel)0), FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var vertical_c_child2 = new Span { Name = "X", Text = "X", Style = new() { Border = new(BorderSize, 0, Color.Red),   FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var vertical_c_child3 = new Span { Name = "Y", Text = "Y", Style = new() { Border = new(BorderSize, 0, Color.Green), FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        var vertical_c_child4 = new Span { Name = "Z", Text = "Z", Style = new() { Border = new(BorderSize, 0, Color.Blue),  FontSize = 48, FontFamily = "Verdana",        Color = Color.Blue } };
        var vertical_c_child5 = new Span { Text = "Hello",         Style = new() { Border = new(BorderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentType.Left,  Margin = new((Pixel)5) } };
        var vertical_c_child6 = new Span { Text = "World!!!",      Style = new() { Border = new(BorderSize, 0, Color.Margenta), Color = Color.White, Alignment = AlignmentType.Right, Margin = new((Pixel)5) } };

        var vertical_d_container = new Span
        {
            Name  = "vertical_c_container",
            Style = new()
            {
                Border     = new(BorderSize, 0, Color.Margenta),
                Color      = Color.White,
                FontFamily = "Impact",
                FontSize   = 24,
                Stack      = StackKind.Vertical,
            }
        };

        var vertical_d_child1 = new Span
        {
            Name  = "vertical_d_child1",
            Style = new()
            {
                Size   = new((Pixel)100),
                Margin = new((Pixel)10),
                Border = new(BorderSize, 0, Color.Cyan),
                Alignment = AlignmentType.Top,
            }
        };

        var vertical_d_child2 = new Span
        {
            Name  = "vertical_d_child2",
            Style = new()
            {
                Size   = new((Pixel)100),
                Margin = new((Pixel)10),
                Border = new(BorderSize, 0, Color.Cyan),
                Alignment = AlignmentType.Bottom,
               
            }
        };

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
