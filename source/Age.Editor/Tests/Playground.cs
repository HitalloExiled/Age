using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class Playground : Element
{
    public override string NodeName { get; } = nameof(Playground);

    public static void Setup(Canvas canvas, in TestContext testContext)
    {
        var root = new FlexBox()
        {
            Name  = "Root",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                Border    = new(1, 0, Color.Red),
                Size      = new((Pixel)200, (Pixel)200),
            }
        };

        var a = new FlexBox()
        {
            Name  = "A[Percentage]",
            // Text  = "Text",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                // Align     = new(1, 1),
                // Margin    = new(0),
                // Baseline  = 1,
                Color     = Color.White,
                Border    = new(1, 0, Color.Green),
                Size      = new((Percentage)50, (Percentage)50),
            }
        };

        var b = new FlexBox()
        {
            Name  = "B[Percentage]",
            // Text  = "Text",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                // Align     = new(1, 1),
                // Margin    = new(0),
                // Baseline  = 1,
                Color     = Color.White,
                Border    = new(1, 0, Color.Blue),
                Size      = new((Percentage)50, (Percentage)50),
            }
        };

        // var a = new Span()
        // {
        //     Text  = "It's Magic!!!",
        //     Name  = "A",
        //     Style = new()
        //     {
        //         FontSize = 48,
        //         Border   = new()
        //         {
        //             Top    = new(20, Color.Red),
        //             Right  = new(20, Color.Green),
        //             Bottom = new(20, Color.Blue),
        //             Left   = new(20, Color.Yellow),
        //             Radius = new(50),
        //         },
        //         // Alignment       = AlignmentType.Top,
        //         BackgroundColor = new Color(1, 0, 1, 0.25f),
        //         // Size            = new(400, 200),
        //         // Margin          = new(50),
        //     }
        // };

        // root.AppendChild(a);

        // a.Blured     += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] Blured");
        // a.Clicked    += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] Clicked");
        // a.Context    += (in ContextEvent contextEvent) => Console.WriteLine($"[{contextEvent.Target.Name}] ContextMenu");
        // a.Focused    += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] Focused");
        // a.MouseMoved += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] MouseMoved");
        // a.MouseOut   += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] MouseOut");
        // a.MouseOver  += (in MouseEvent mouseEvent)     => Console.WriteLine($"[{mouseEvent.Target.Name}] MouseOver");

        // var b = new Span()
        // {
        //     Text  = "It's Magic!!!",
        //     Name  = "B",
        //     Style = new()
        //     {
        //         FontSize = 48,
        //         Border   = new()
        //         {
        //             Top    = new(20, Color.Red),
        //             Right  = new(20, Color.Green),
        //             Bottom = new(20, Color.Blue),
        //             Left   = new(20, Color.Yellow),
        //             Radius = new(0, 20),
        //         },
        //         BoxSizing       = BoxSizing.Border,
        //         // Alignment       = AlignmentType.Top,
        //         BackgroundColor = new Color(1, 0, 1, 0.25f),
        //         // Position        = new(100, -100),
        //         // Size            = new(400, 200),
        //         // Margin          = new(50),
        //     }
        // };

        // var c = new Span()
        // {
        //     Text  = "C...",
        //     Name  = "C",
        //     Style = new()
        //     {
        //         FontSize  = 24,
        //         Border    = new(4, 10, Color.Blue),
        //         Size      = SizeUnit.Pixel(100, 50),
        //         // Alignment = AlignmentType.Top,
        //     }
        // };

        // root.AppendChild(a);
        // root.AppendChild(b);
        // root.AppendChild(c);


        // root.AppendChild(new Boxes());
        // root.AppendChild(new Boxes());
        // root.AppendChild(new Boxes());

        // var parentSpan = new Span()
        // {
        //     Name  = "Parent",
        //     Text  = "Text",
        //     Style = new()
        //     {
        //         // Align      = new(),
        //         FontSize   = 24,
        //         FontFamily = "Impact",
        //         // Size       = new(50),
        //     }
        // };

        // root.AppendChild(parentSpan);

        // var childSpan1 = new Span() { Name = "X", Text = "X", Style = new() { FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        // var childSpan2 = new Span() { Name = "Y", Text = "Y", Style = new() { FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        // var childSpan3 = new Span() { Name = "Z", Text = "Z", Style = new() { FontSize = 48, FontFamily = "Verdana", Color = Color.Blue } };
        // var childSpan4 = new Span() { Text = "Hello",         Style = new() { Alignment = AlignmentType.Top, Margin = new(4) } };
        // var childSpan5 = new Span() { Text = "World!!!",      Style = new() { Alignment = AlignmentType.Bottom, Margin = new(4) } };

        // parentSpan.AppendChild(childSpan1);
        // parentSpan.AppendChild(childSpan2);
        // parentSpan.AppendChild(childSpan3);
        // parentSpan.AppendChild(childSpan4);
        // parentSpan.AppendChild(childSpan5);

        canvas.AppendChild(root);
            root.AppendChild(a);
                a.AppendChild(b);
    }
}
