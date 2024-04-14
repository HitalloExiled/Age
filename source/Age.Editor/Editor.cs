using Age.Core;
using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Drawing.Styling;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas  canvas;
    private readonly Span    statusText;

    private ulong  frames;
    private double minFps = double.MaxValue;
    private double maxFps;
    private double totalFps;
    private double maxFrameTime;
    private double minFrameTime = double.MaxValue;
    private double delta;
    private bool   increasing = true;
    private double timeElapsed;

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
                Margin    = new(20),
                Alignment = AlignmentType.Center | AlignmentType.Top
            }
        };

        this.canvas.AppendChild(root);

        this.statusText = new Span()
        {
            Name  = "Status",
            Text  =
            """
            Frame
            """,
            Style = style with
            {
                Margin      = new(10),
                BorderColor = Color.Red,
                Color       = Color.Margenta,
                FontSize    = 24,
            }
        };

        root.AppendChild(this.statusText);

        var parentSpan = new Span()
        {
            Name  = "Parent",
            Text  = "Text",
            Style = style with
            {
                FontSize   = 48,
                FontFamily = "Impact",
            }
        };

        root.AppendChild(parentSpan);

        var childSpan1 = new Span() { Name = "X", Text = "X", Style = style with { FontSize = 48, FontFamily = "Helvetica Neue", Color = Color.Red } };
        var childSpan2 = new Span() { Name = "Y", Text = "Y", Style = style with { FontSize = 24, FontFamily = "Lucida Console", Color = Color.Green } };
        var childSpan3 = new Span() { Name = "Z", Text = "Z", Style = style with { FontSize = 48, FontFamily = "Verdana", Color = Color.Blue } };
        var childSpan4 = new Span() { Text = "Hello",         Style = style with { Margin = new(4, 0) } };
        var childSpan5 = new Span() { Text = "World!!!",      Style = style with { Margin = new(4, 0) } };

        parentSpan.AppendChild(childSpan1);
        parentSpan.AppendChild(childSpan2);
        parentSpan.AppendChild(childSpan3);
        parentSpan.AppendChild(childSpan4);
        parentSpan.AppendChild(childSpan5);
    }

    protected override void OnInitialize() =>
        Logger.Debug(this.statusText.Size.ToString());

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

        this.timeElapsed += deltaTime * 1000;

        // if (this.timeElapsed > 16.66)
        {
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

                Size: {this.statusText.Size};
                """;

            this.timeElapsed = 0;
        }

        this.frames++;
    }
}
