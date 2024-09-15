using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Editor.Tests;

namespace Age.Editor;

internal delegate void Setup(Canvas canvas, in TestContext context);

public class Editor : Node
{
    private const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();

    private Setup setup;

    private TestContext testContext;

    private float borderSize = 10;
    private float scale      = 1;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        // this.canvas.AppendChild(new FrameStatus());
        this.setup = BoxModelTest.Setup;
        //this.setup = Tests.MarginTest.Setup;
        //this.setup = Tests.PaddingTest.Setup;
        //this.setup = DivTest.Setup;

        this.testContext = new()
        {
            BorderSize = (uint)this.borderSize,
            Scale      = this.scale,
        };

        this.Reload();
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
                Alignment = AlignmentKind.Center,
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

        var sideViews = new Span() { Style = new() { Stack = StackKind.Vertical, Alignment = AlignmentKind.Center } };

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
        var borderSize = float.Ceiling(this.borderSize);
        var scale      = this.scale;

        var reload = Input.IsKeyPressed(Key.Control) && Input.IsKeyJustPressed(Key.R);

        if (Input.IsKeyJustPressed(Key.Num1))
        {
            this.setup = BoxModelTest.Setup;
            reload = true;
        }
        else if (Input.IsKeyJustPressed(Key.Num2))
        {
            this.setup = BoxSizingTest.Setup;
            reload = true;
        }
        else if (Input.IsKeyJustPressed(Key.Num3))
        {
            this.setup = DivTest.Setup;
            reload = true;
        }
        else if (Input.IsKeyJustPressed(Key.Num4))
        {
            this.setup = InlineTextTest.Setup;
            reload = true;
        }
        else if (Input.IsKeyJustPressed(Key.Num5))
        {
            this.setup = MarginTest.Setup;
            reload = true;
        }
        else if (Input.IsKeyJustPressed(Key.Num6))
        {
            this.setup = PaddingTest.Setup;
            reload = true;
        }

        if (Input.IsKeyPressed(Key.Add))
        {
            if (Input.IsKeyPressed(Key.Control))
            {
                this.borderSize = float.Min(100, this.borderSize + 10 * (float)deltaTime);

                borderSize = (uint)float.Ceiling(this.borderSize);
            }
            else
            {
                this.scale = float.Min(100, this.scale + 10 * (float)deltaTime);
            }

            reload = true;
        }

        if (Input.IsKeyPressed(Key.Subtract))
        {
            if (Input.IsKeyPressed(Key.Control))
            {
                this.borderSize = float.Max(0, this.borderSize - 10 * (float)deltaTime);

                borderSize = (uint)float.Ceiling(this.borderSize);
            }
            else
            {
                this.scale = float.Max(1, this.scale - 10 * (float)deltaTime);
            }

            reload = true;
        }

        if (reload)
        {
            this.testContext.BorderSize = (uint)borderSize;
            this.testContext.Scale      = scale;

            this.Reload();
        }
    }

    private void Reload()
    {
        this.canvas.RemoveChildren();

        this.setup.Invoke(this.canvas, this.testContext);
    }

#if DEBUG
    protected override void Connected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated += this.Reload;

    protected override void Disconnected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated -= this.Reload;
#endif

    public override void Update(double deltaTime) =>
        this.HandleBorders(deltaTime);
}
