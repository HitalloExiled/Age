using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;

namespace Age.Editor;

public class Editor : Node
{
    const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        // this.canvas.AppendChild(new BoxModel());
        this.CreateDemoScene();
    }

    private void CreateDemoScene()
    {
        var root = new Span
        {
            Name = "Root",
            Style = new()
            {
                Size = SizeUnit.Percentage(100),
                Border = new(BORDER_SIZE, default, Color.Margenta),
            }
        };

        var verticalStack = new Span()
        {
            Name  = "VStack",
            Style = new()
            {
                Stack  = StackType.Vertical,
                Size   = SizeUnit.Percentage(100),
                Border = new(BORDER_SIZE, default, Color.Yellow),
            }
        };

        var header = new Span
        {
            Name  = "Header",
            Style = new()
            {
                Size   = new(Unit.Percentage(100), null),
                Border = new(BORDER_SIZE, default, Color.Red),
            }
        };

        var content = new Span
        {
            Name  = "Content",
            Style = new()
            {
                Size   = new(Unit.Percentage(100)),
                Border = new(BORDER_SIZE, default, Color.Green),
            }
        };

        var viewports = new Span
        {
            Name  = "Viewports",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Border    = new(BORDER_SIZE, default, Color.Blue),
                Size      = new(400),
            }
        };

        // this.canvas.AppendChild(new Playground());

        // var scene = new DemoScene();

        // var freeViewport  = new Viewport(new(300)) { Name = "Red" };
        // var redViewport   = new Viewport(new(100)) { Name = "Red" };
        // var greenViewport = new Viewport(new(100)) { Name = "Green" };
        // var blueViewport  = new Viewport(new(100)) { Name = "Blue" };

        // freeViewport.Style.Border  = new(1, 0, Color.White);
        // redViewport.Style.Border   = new(1, 0, Color.Red);
        // greenViewport.Style.Border = new(1, 0, Color.Green);
        // blueViewport.Style.Border  = new(1, 0, Color.Blue);

        // freeViewport.Style.BoxSizing = redViewport.Style.BoxSizing = greenViewport.Style.BoxSizing = blueViewport.Style.BoxSizing = BoxSizing.Border;

        // scene.FreeCamera.RenderTargets.Add(freeViewport.RenderTarget);
        // scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        // scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        // scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        var sideViews = new Span() { Style = new() { Stack = StackType.Vertical, Alignment = AlignmentType.Center } };

        this.canvas.AppendChild(root);
        // this.AppendChild(scene);

            root.AppendChild(verticalStack);

                verticalStack.AppendChild(header);
                    header.AppendChild(new FrameStatus());

                verticalStack.AppendChild(content);
                    // content.AppendChild(viewports);

                        // viewports.AppendChild(freeViewport);
                        // viewports.AppendChild(sideViews);

                            // sideViews.AppendChild(redViewport);
                            // sideViews.AppendChild(greenViewport);
                            // sideViews.AppendChild(blueViewport);

    }
}
