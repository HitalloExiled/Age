using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas canvas = new();

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.canvas.AppendChild(new BoxModel());
        // this.CreateDemoScene();
    }

    private void CreateDemoScene()
    {
        var verticalStack = new Span() { Style = new() { Stack = StackType.Vertical } };
        var header        = new Span() { Name = "Header" };
        var viewports     = new Span() { Name = "Viewports", Style = new() { Alignment = AlignmentType.Center } };

        this.canvas.AppendChild(verticalStack);

        verticalStack.AppendChild(header);
        verticalStack.AppendChild(viewports);

        header.AppendChild(new FrameStatus());
        // this.canvas.AppendChild(new Playground());

        var scene = new DemoScene();

        var freeViewport  = new Viewport(new(600)) { Name = "Red" };
        var redViewport   = new Viewport(new(200)) { Name = "Red" };
        var greenViewport = new Viewport(new(200)) { Name = "Green" };
        var blueViewport  = new Viewport(new(200)) { Name = "Blue" };

        freeViewport.Style.Border  = new(1, 0, Color.White);
        redViewport.Style.Border   = new(1, 0, Color.Red);
        greenViewport.Style.Border = new(1, 0, Color.Green);
        blueViewport.Style.Border  = new(1, 0, Color.Blue);

        freeViewport.Style.BoxSizing = redViewport.Style.BoxSizing = greenViewport.Style.BoxSizing = blueViewport.Style.BoxSizing = BoxSizing.Border;

        scene.FreeCamera.RenderTargets.Add(freeViewport.RenderTarget);
        scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        var sideViews = new Span() { Style = new() { Stack = StackType.Vertical, Alignment = AlignmentType.Center } };

        sideViews.AppendChild(redViewport);
        sideViews.AppendChild(greenViewport);
        sideViews.AppendChild(blueViewport);

        viewports.AppendChild(freeViewport);
        viewports.AppendChild(sideViews);

        this.AppendChild(scene);
    }
}
