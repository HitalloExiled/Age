using Age.Numerics;
using Age.Rendering.Drawing;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Text statusText;

    private ulong  frames       = 0ul;
    private double minFps       = double.MaxValue;
    private double maxFps       = 0.0;
    private double totalFps     = 0.0;
    private double maxFrameTime = 0.0;
    private double minFrameTime = double.MaxValue;

    public Editor()
    {
        var style = new Style
        {
            Border = new(),
        };

        statusText = new Text("", style with { FontSize = 24, Color = Color.Margenta, /* Position = new(4, -4) */ });

        this.Add(statusText);

        // this.Add(new Text("Hello\nWorld\n!!!", style with { FontSize = 100, Color = Color.Green, /* Position = new(100, -200) */ }));
        // this.Add(new Text("Hello World!!!",    style with { FontSize = 50,  Color = Color.Blue,  /* Position = new(50,  -500) */ }));
    }

    protected override void OnUpdate(double deltaTime)
    {
        var fps       = Math.Round(1 / deltaTime, 2);
        var frameTime = Math.Round(deltaTime * 1000, 2);
        var avgFps    = Math.Round(totalFps / frames, 2);

        totalFps += fps;

        maxFps = Math.Max(maxFps, fps);
        minFps = Math.Min(minFps, fps);

        maxFrameTime = Math.Max(maxFrameTime, frameTime);
        minFrameTime = Math.Min(minFrameTime, frameTime);

        statusText.Value =
            $"""
            Frames:    {frames}
            DeltaTime: {Math.Round(deltaTime, 4)}
            FPS: {fps}
                Avg: {avgFps}
                Min: {minFps}
                Max: {maxFps}

            FrameTime: {frameTime}ms
                Min: {minFrameTime}ms
                Max: {maxFrameTime}ms
            """;

        frames++;
    }
}
