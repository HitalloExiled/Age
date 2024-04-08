using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas canvas;
    private readonly Span   statusText;

    private ulong  frames;
    private double minFps = double.MaxValue;
    private double maxFps;
    private double totalFps;
    private double maxFrameTime;
    private double minFrameTime = double.MaxValue;
    private double delta;
    private bool   increasing = true;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        var style = new Style
        {
            Border = new(),
            Font   = new() { Size = 24 },
        };

        this.AppendChild(this.canvas = new Canvas());

        var root = new Span() { Name = "Root", Style = new() { Position = new(8, -8) } };

        this.canvas.AppendChild(root);

        this.statusText = new Span()
        {
            Name  = "Status",
            Style = style with
            {
                Color = Color.Margenta,
                /* Position = new(-10, 10) */
            }
        };

        // root.AppendChild(this.statusText);

        // // this.Append(new Text("Hello\nWorld\n!!!", style with { FontSize = 100, Color = Color.Green, /* Position = new(100, -200) */ }));
        // // this.Append(new Text("Hello World!!!",    style with { FontSize = 50,  Color = Color.Blue,  /* Position = new(50,  -500) */ }));
        var parentSpan = new Span() { Name = "Parent", Text = "Text", Style = new() { Font = new() { Size = 16 }, /* Position = new(20, -20) */ } };
        root.AppendChild(parentSpan);

        var childSpan1 = new Span() { Name = "X", Text = "X", Style = new() { Font = new() { Size = 24 }, /* Size = new(100, 100), *//* Position = new(0, -10), */  Color = Color.Red } };
        var childSpan2 = new Span() { Name = "Y", Text = "Y", Style = new() { Font = new() { Size = 48 }, /* Size = new(100, 100), *//* Position = new(0, 0),   */  Color = Color.Green } };
        var childSpan3 = new Span() { Name = "Z", Text = "Z", Style = new() { Font = new() { Size = 24 }, /* Size = new(100, 100), *//* Position = new(0, 10),  */  Color = Color.Blue } };

        parentSpan.AppendChild(childSpan1);
        parentSpan.AppendChild(childSpan2);
        parentSpan.AppendChild(childSpan3);
    }

    protected override void OnUpdate(double deltaTime)
    {
        this.delta = this.increasing
            ? double.Min(this.delta + deltaTime, 1)
            : double.Max(this.delta - deltaTime, -1);

        if (this.increasing && this.delta == 1)
        {
            this.increasing = false;
        }
        else if (!this.increasing && this.delta == -1)
        {
            this.increasing = true;
        }

        var fps       = Math.Round(1 / deltaTime, 2);
        var frameTime = Math.Round(deltaTime * 1000, 2);
        var avgFps    = Math.Round(this.totalFps / this.frames, 2);

        this.totalFps += fps;

        this.maxFps = Math.Max(this.maxFps, fps);
        this.minFps = Math.Min(this.minFps, fps);

        this.maxFrameTime = Math.Max(this.maxFrameTime, frameTime);
        this.minFrameTime = Math.Min(this.minFrameTime, frameTime);

        this.statusText.Style.Position = new Point<int>((int)(double.Cos(this.delta) * 50), (int)(double.Sin(this.delta) * -50));

        this.statusText.Text =
            $"""
            Frames:    {this.frames}
            DeltaTime: {Math.Round(deltaTime, 4)}
            FPS: {fps}
                Avg: {avgFps}
                Min: {this.minFps}
                Max: {this.maxFps}

            FrameTime: {frameTime}ms
                Min: {this.minFrameTime}ms
                Max: {this.maxFrameTime}ms

            Position: {this.statusText.Style.Position};
            """;

        this.frames++;
    }
}
