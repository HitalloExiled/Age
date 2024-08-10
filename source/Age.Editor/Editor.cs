using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas    canvas = new();
    // private readonly DemoScene scene;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);

        var a = new Span()
        {
            Name  = "A",
            Style = new()
            {
                Color  = Color.White,
                Border = new(1, 0, Color.Red),
                Size   = new(Unit.Pixel(100), Unit.Pixel(100)),
            }
        };

        var b = new Span()
        {
            Name  = "B",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Color  = Color.White,
                Border = new(1, 0, Color.Green),
                Size   = new(Unit.Percentage(50), Unit.Percentage(50)),
            }
        };

        var c = new Span()
        {
            Name  = "C",
            Style = new()
            {
                Color  = Color.White,
                Border = new(1, 0, Color.Blue),
                Size   = new(Unit.Percentage(50), Unit.Percentage(50)),
            }
        };

        var d = new Span()
        {
            Name  = "D",
            Style = new()
            {
                Color  = Color.White,
                Border = new(1, 0, Color.White),
                Size   = new(Unit.Percentage(50), Unit.Percentage(50)),
            }
        };

        this.canvas.AppendChild(a);
        a.AppendChild(b);
        b.AppendChild(c);
        c.AppendChild(d);

        // var verticalStack = new Span() { Style = new() { Stack = StackType.Vertical } };
        // var header        = new Span() { Name = "Header" };
        // var viewports     = new Span() { Name = "Viewports", Style = new() { Alignment = AlignmentType.Center } };

        // this.canvas.AppendChild(verticalStack);

        // verticalStack.AppendChild(header);
        // verticalStack.AppendChild(viewports);

        // header.AppendChild(new FrameStatus());
        // this.canvas.AppendChild(new Playground());

        // this.scene = new DemoScene();

        // var freeViewport  = new Viewport(new(600)) { Name = "Red" };
        // var redViewport   = new Viewport(new(200)) { Name = "Red" };
        // var greenViewport = new Viewport(new(200)) { Name = "Green" };
        // var blueViewport  = new Viewport(new(200)) { Name = "Blue" };

        // freeViewport.Style.Border  = new(1, 0, Color.White);
        // redViewport.Style.Border   = new(1, 0, Color.Red);
        // greenViewport.Style.Border = new(1, 0, Color.Green);
        // blueViewport.Style.Border  = new(1, 0, Color.Blue);

        // freeViewport.Style.BoxSizing = redViewport.Style.BoxSizing = greenViewport.Style.BoxSizing = blueViewport.Style.BoxSizing = BoxSizing.Border;

        // this.scene.FreeCamera.RenderTargets.Add(freeViewport.RenderTarget);
        // this.scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        // this.scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        // this.scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        // var sideViews = new Span() { Style = new() { Stack = StackType.Vertical, Alignment = AlignmentType.Center } };

        // sideViews.AppendChild(redViewport);
        // sideViews.AppendChild(greenViewport);
        // sideViews.AppendChild(blueViewport);

        // viewports.AppendChild(freeViewport);
        // viewports.AppendChild(sideViews);

        // this.AppendChild(this.scene);
    }

    // protected override void OnUpdate(double deltaTime) =>
    //     this.status.Text =
    //     $"""
    //     FrontCamera: {this.scene.FrontCamera.Transform}
    //     SideCamera:  {this.scene.SideCamera.Transform}
    //     TopCamera:   {this.scene.TopCamera.Transform}
    //     """;
}
