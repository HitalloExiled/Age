using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;

namespace Age.Editor;

public class FrameStatus : Element
{
    public override string NodeName { get; } = nameof(FrameStatus);

    private readonly Span statusText;

    private double delta;
    private ulong  frames;
    private bool   increasing = true;
    private double maxFps;
    private double maxFrameTime;
    private double minFps = double.MaxValue;
    private double minFrameTime = double.MaxValue;
    private double totalFps;

    public FrameStatus()
    {
        this.statusText = new Span()
        {
            Name  = "Status",
            Style = new()
            {
                Color    = Color.Margenta,
                FontSize = 12,
                MinSize  = new(110, 30),
                BackgroundColor = new Color(1, 1, 0)
            }
        };

        this.AppendChild(this.statusText);
    }

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
