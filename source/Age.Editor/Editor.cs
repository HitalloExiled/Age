using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Scene;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas canvas = new();
    // private readonly Span   status;
    private readonly DemoScene scene;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.canvas.AppendChild(new FrameStatus());
        // this.canvas.AppendChild(this.status = new() { Style = new() { Color = Color.Green } });

        this.scene = new DemoScene();

        var redViewport   = new Viewport(new(200));
        var greenViewport = new Viewport(new(200));
        var blueViewport  = new Viewport(new(200));

        redViewport.Style.Border   = new(1, 0, Color.Red);
        greenViewport.Style.Border = new(1, 0, Color.Green);
        blueViewport.Style.Border  = new(1, 0, Color.Blue);

        this.scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        this.scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        this.scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        this.canvas.AppendChild(redViewport);
        this.canvas.AppendChild(greenViewport);
        this.canvas.AppendChild(blueViewport);
        this.canvas.AppendChild(this.scene);
    }

    // protected override void OnUpdate(double deltaTime) =>
    //     this.status.Text =
    //     $"""
    //     FrontCamera: {this.scene.FrontCamera.Transform}
    //     SideCamera:  {this.scene.SideCamera.Transform}
    //     TopCamera:   {this.scene.TopCamera.Transform}
    //     """;
}
