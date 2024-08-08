using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Drawing.Styling;
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

        var verticalStack = new Span() { Style = new() { Stack = StackType.Vertical } };
        var header        = new Span() { Name = "Header" };
        var viewports     = new Span() { Name = "Viewports" };

        this.canvas.AppendChild(verticalStack);

        verticalStack.AppendChild(header);
        verticalStack.AppendChild(viewports);

        header.AppendChild(new FrameStatus());
        header.AppendChild(new Playground() { Style = new() { Alignment = AlignmentType.Bottom } });

        this.scene = new DemoScene();

        var redViewport   = new Viewport(new(200)) { Name = "Red" };
        var greenViewport = new Viewport(new(200)) { Name = "Green" };
        var blueViewport  = new Viewport(new(200)) { Name = "Blue" };

        redViewport.Style.Border   = new(1, 0, Color.Red);
        greenViewport.Style.Border = new(1, 0, Color.Green);
        blueViewport.Style.Border  = new(1, 0, Color.Blue);

        this.scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        // this.scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        this.scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        viewports.AppendChild(redViewport);
        viewports.AppendChild(greenViewport);
        viewports.AppendChild(blueViewport);
        this.AppendChild(this.scene);
    }

    // protected override void OnUpdate(double deltaTime) =>
    //     this.status.Text =
    //     $"""
    //     FrontCamera: {this.scene.FrontCamera.Transform}
    //     SideCamera:  {this.scene.SideCamera.Transform}
    //     TopCamera:   {this.scene.TopCamera.Transform}
    //     """;
}
