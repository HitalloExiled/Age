using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Drawing.Styling;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas canvas;
    private readonly Span   statusText;

    private double delta;
    private ulong  frames;
    private bool   increasing = true;
    private double maxFps;
    private double maxFrameTime;
    private double minFps = double.MaxValue;
    private double minFrameTime = double.MaxValue;
    private double totalFps;
    private readonly Element a;
    private readonly Element b;
    private readonly Element c;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        var style = new Style()
        {
            FontSize = 24,
        };

        this.AppendChild(this.canvas = new Canvas());

        var root = new Span()
        {
            Name  = "Root",
            Style = new()
            {
                // Baseline = 0,
                FontSize  = 24,
                Alignment = AlignmentType.Center,
            }
        };

        this.canvas.AppendChild(root);

        this.a = new Span()
        {
            Text  = "It's Magic!!!",
            Name  = "A",
            Style = new()
            {
                FontSize = 48,
                Border   = new()
                {
                    Top    = new(20, Color.Red),
                    Right  = new(20, Color.Green),
                    Bottom = new(20, Color.Blue),
                    Left   = new(20, Color.Yellow),
                    Radius = new(0, 20),
                },
                BackgroundColor = new Color(1, 0, 1, 0.25f),
                // Size            = new(400, 200),
                Margin          = new(50),
            }
        };

        this.b = new Span()
        {
            Text  = "It's Magic!!!",
            Name  = "A",
            Style = new()
            {
                FontSize = 48,
                Border   = new()
                {
                    Top    = new(20, Color.Red),
                    Right  = new(20, Color.Green),
                    Bottom = new(20, Color.Blue),
                    Left   = new(20, Color.Yellow),
                    Radius = new(0, 20),
                },
                BoxSizing       = BoxSizing.Border,
                BackgroundColor = new Color(1, 0, 1, 0.25f),
                // Size            = new(400, 200),
                Margin          = new(50),
            }
        };

        // this.b.LocalTransform = this.b.LocalTransform with { Scale = new(0.5f) };
        // this.b.Pivot = new(300, -50);

        this.c = new Span()
        {
            Text  = "C...",
            Name  = "C",
            Style = new()
            {
                FontSize = 24,
                Border   = new(2, 20, Color.Blue),
                Size     = new(100, 50),
            }
        };

        // this.c.LocalTransform = this.c.LocalTransform with { /* Rotation = RADIANS * 45,  */Position = new(100, 0) };
        this.c.Style.Pivot    = new(-1, 1);
        this.c.Style.Align    = new(1, -1);
        this.c.Style.Position = new(50, 50);

        root.AppendChild(this.a);
        // // this.canvas.AppendChild(b);
        root.AppendChild(this.b);
        // this.b.AppendChild(this.c);

        this.statusText = new Span()
        {
            Name  = "Status",
            Text  =
            """
            Frame
            Frame
            """,
            Style = style with
            {
                // Align       = new(),
                Border   = new(1, 0, Color.Red),
                Color    = Color.Margenta,
                FontSize = 24,
                Margin   = new(10),
                // Size        = new(100),
            }
        };

        // root.AppendChild(this.statusText);
        // root.AppendChild(new Boxes());

        // var parentSpan = new Span()
        // {
        //     Name  = "Parent",
        //     Text  = "Text",
        //     Style = style with
        //     {
        //         // Align      = new(),
        //         FontSize   = 24,
        //         FontFamily = "Impact",
        //         // Size       = new(50),
        //     }
        // };

        // root.AppendChild(parentSpan);

        // var childSpan1 = new Span() { Name = "X", Text = "X", Style = style with { FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        // var childSpan2 = new Span() { Name = "Y", Text = "Y", Style = style with { FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        // var childSpan3 = new Span() { Name = "Z", Text = "Z", Style = style with { FontSize = 48, FontFamily = "Verdana", Color = Color.Blue } };
        // var childSpan4 = new Span() { Text = "Hello",         Style = style with { Alignment = AlignmentType.Top, Margin = new(4) } };
        // var childSpan5 = new Span() { Text = "World!!!",      Style = style with { Alignment = AlignmentType.Bottom, Margin = new(4) } };

        // parentSpan.AppendChild(childSpan1);
        // parentSpan.AppendChild(childSpan2);
        // parentSpan.AppendChild(childSpan3);
        // parentSpan.AppendChild(childSpan4);
        // parentSpan.AppendChild(childSpan5);
    }

    protected override void OnInitialize() =>
        Console.WriteLine(this.statusText.Size);

    protected override void OnUpdate(double deltaTime)
    {
        this.delta = this.increasing
            ? double.Min(this.delta + deltaTime * 0.1f, 1)
            : double.Max(this.delta - deltaTime * 0.1f, -1);

        if (this.increasing && this.delta == 1)
        {
            this.increasing = false;
        }
        else if (!this.increasing && this.delta == -1)
        {
            this.increasing = true;
        }

        this.frames++;

        var fps       = Math.Round(1 / deltaTime, 2);
        this.totalFps += fps;

        var frameTime = Math.Round(deltaTime * 1000, 2);
        var avgFps    = Math.Round(this.totalFps / this.frames, 2);

        this.maxFps = Math.Max(this.maxFps, fps);
        this.minFps = Math.Min(this.minFps, fps);

        this.maxFrameTime = Math.Max(this.maxFrameTime, frameTime);
        this.minFrameTime = Math.Min(this.minFrameTime, frameTime);

        this.b.LocalTransform = this.b.LocalTransform with { Rotation = Angle.Radians<float>(90) * (float)this.delta };
        // this.c.LocalTransform = this.c.LocalTransform with {  Rotation = RADIANS * 45 * (float)this.delta };
        // this.b.Style.Rotation = RADIANS * 90 * (float)this.delta;
        this.c.Style.Rotation = Angle.Radians<float>(45) * (float)this.delta;

        // this.statusText.LocalTransform = this.statusText.LocalTransform with { Position = new Point<int>((int)(double.Cos(this.delta) * 50), (int)(double.Sin(this.delta) * -50)) };
        this.statusText.Text =
            $"""
            Frames:    {this.frames}
            Delta Time: {Math.Round(deltaTime, 4)}
            FPS: {fps}
                Avg: {avgFps}
                Min: {this.minFps}
                Max: {this.maxFps}

            Frame Time: {frameTime}ms
                Min: {this.minFrameTime}ms
                Max: {this.maxFrameTime}ms
            """;
    }
}
