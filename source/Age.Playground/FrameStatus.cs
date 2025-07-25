using Age.Elements;
using Age.Numerics;
using Age.Scene;

namespace Age.Playground;

public class FrameStatus : Element
{
    public override string NodeName => nameof(FrameStatus);

    private readonly FlexBox statusText;

    private double delta;
    private bool   increasing = true;
    private double maxFps;
    private double maxFrameTime;
    private double minFps = double.MaxValue;
    private double minFrameTime = double.MaxValue;
    private double totalFps;

    public bool Enabled { get; set; } = true;

    public FrameStatus()
    {
        this.statusText = new FlexBox()
        {
            Name      = "Status",
            InnerText = "Frame",
            Style     = new()
            {
                Color    = Color.Green,
                FontSize = 16,
                Border   = new(10, 0, Color.Green),
                Padding  = new(Unit.Px(10)),
                MinSize  = new(Unit.Px(150), Unit.Px(162)),
                // BackgroundColor = new Color(1, 1, 0)
            }
        };

        this.AppendChild(this.statusText);
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        base.OnConnected(renderTree);

        renderTree.Updated += this.Update;
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        renderTree.Updated -= this.Update;
    }

    public override void Update()
    {
        this.delta = this.increasing
            ? double.Min(this.delta + (Time.DeltaTime * 0.1f), 1)
            : double.Max(this.delta - (Time.DeltaTime * 0.1f), -1);

        if (this.increasing && this.delta == 1)
        {
            this.increasing = false;
        }
        else if (!this.increasing && this.delta == -1)
        {
            this.increasing = true;
        }

        var fps = double.Round(Time.Fps, 2);

        this.totalFps += fps;

        var frameTime = Math.Round(Time.DeltaTime * 1000, 2);
        var avgFps    = Math.Round(this.totalFps / Time.Frames, 2);

        this.maxFps = Math.Max(this.maxFps, fps);
        this.minFps = Math.Min(this.minFps, fps);

        this.maxFrameTime = Math.Max(this.maxFrameTime, frameTime);
        this.minFrameTime = Math.Min(this.minFrameTime, frameTime);

        if (this.Enabled)
        {
            this.statusText.InnerText =
               $"""
               Frames:    {Time.Frames}
               Delta Time: {Math.Round(Time.DeltaTime, 4)}
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
}
