using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Editor;

public class Editor : Node
{
    private const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();
    private float borderSize = Tests.InlineText.BorderSize;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        // this.canvas.AppendChild(new FrameStatus());
        // Tests.BoxModelTest.Setup(this.canvas);
        // Tests.MarginTest.Setup(this.canvas);
        // Tests.PaddingTest.Setup(this.canvas);
        Tests.InlineText.Setup(this.canvas);
        // this.CreateDemoScene();
    }

    private void CreateDemoScene()
    {
        var root = new Span
        {
            Name  = "Root",
            Style = new()
            {
                Size   = new((Percentage)100),
                Border = new(BORDER_SIZE, default, Color.Margenta),
            }
        };

        var verticalStack = new Span()
        {
            Name  = "VStack",
            Style = new()
            {
                Stack  = StackKind.Vertical,
                Size   = new((Percentage)100),
                Border = new(BORDER_SIZE, default, Color.Yellow),
            }
        };

        var header = new Span
        {
            Name  = "Header",
            Style = new()
            {
                Size   = new((Percentage)100, null),
                Border = new(BORDER_SIZE, default, Color.Red),
            }
        };

        var content = new Span
        {
            Name  = "Content",
            Style = new()
            {
                Size   = new((Percentage)100),
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
                Size      = new((Pixel)400),
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

        var sideViews = new Span() { Style = new() { Stack = StackKind.Vertical, Alignment = AlignmentType.Center } };

        this.canvas.AppendChild(root);
        // this.AppendChild(scene);

            root.AppendChild(verticalStack);

                verticalStack.AppendChild(header);
                    header.AppendChild(new FrameStatus());

                verticalStack.AppendChild(content);
                    content.AppendChild(viewports);

                        // viewports.AppendChild(freeViewport);
                        // viewports.AppendChild(sideViews);

                        //     sideViews.AppendChild(redViewport);
                        //     sideViews.AppendChild(greenViewport);
                        //     sideViews.AppendChild(blueViewport);

    }

    private void HandleBorders(double deltaTime)
    {
        var borderSize = Tests.InlineText.BorderSize; float.Ceiling(this.borderSize);

        if (Input.IsKeyPressed(Key.Add))
        {
            this.borderSize = float.Min(100, this.borderSize + 10 * (float)deltaTime);

            borderSize = (uint)float.Ceiling(this.borderSize);
        }

        if (Input.IsKeyPressed(Key.Subtract))
        {
            this.borderSize = float.Max(0, this.borderSize - 10 * (float)deltaTime);

            borderSize = (uint)float.Ceiling(this.borderSize);
        }


        if (borderSize != Tests.InlineText.BorderSize || Input.IsKeyPressed(Key.Control) && Input.IsKeyJustPressed(Key.R))
        {
            this.canvas.RemoveChildren();

            Tests.InlineText.BorderSize = borderSize;
            Tests.InlineText.Setup(this.canvas);
        }
    }

    public override void Update(double deltaTime) =>
        this.HandleBorders(deltaTime);
}
