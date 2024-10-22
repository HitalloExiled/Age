using Age.Elements;
using Age.Styling;
using Age.Scene;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Editor.Tests;

namespace Age.Editor;

internal delegate void Setup(Canvas canvas);

public class Editor : Node
{
    private const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();

    private Setup setup;

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.setup = BoxModelTest.Setup;

        this.Reload();
        // this.CreateDemoScene();
    }

    private void CreateDemoScene()
    {
        var root = new FlexBox
        {
            Name  = "Root",
            Style = new()
            {
                Size   = new((Percentage)100),
                Border = new(BORDER_SIZE, default, Color.Margenta),
            }
        };

        var verticalStack = new FlexBox()
        {
            Name  = "VStack",
            Style = new()
            {
                Stack  = StackKind.Vertical,
                Size   = new((Percentage)100),
                Border = new(BORDER_SIZE, default, Color.Yellow),
            }
        };

        var header = new FlexBox
        {
            Name  = "Header",
            Style = new()
            {
                Size   = new((Percentage)100, null),
                Border = new(BORDER_SIZE, default, Color.Red),
            }
        };

        var content = new FlexBox
        {
            Name  = "Content",
            Style = new()
            {
                Size   = new((Percentage)100),
                Border = new(BORDER_SIZE, default, Color.Green),
            }
        };

        var viewports = new FlexBox
        {
            Name  = "Viewports",
            Style = new()
            {
                Alignment = AlignmentKind.Center,
                Border    = new(BORDER_SIZE, default, Color.Blue),
            }
        };

        var scene = new DemoScene();

        var freeViewport  = new Viewport(new(300)) { Name = "Red" };
        var redViewport   = new Viewport(new(100)) { Name = "Red" };
        var greenViewport = new Viewport(new(100)) { Name = "Green" };
        var blueViewport  = new Viewport(new(100)) { Name = "Blue" };

        freeViewport.Style.Border  = new(1, 0, Color.White);
        redViewport.Style.Border   = new(1, 0, Color.Red);
        greenViewport.Style.Border = new(1, 0, Color.Green);
        blueViewport.Style.Border  = new(1, 0, Color.Blue);

        freeViewport.Style.BoxSizing = redViewport.Style.BoxSizing = greenViewport.Style.BoxSizing = blueViewport.Style.BoxSizing = BoxSizing.Border;

        scene.FreeCamera.RenderTargets.Add(freeViewport.RenderTarget);
        scene.RedCamera.RenderTargets.Add(redViewport.RenderTarget);
        scene.GreenCamera.RenderTargets.Add(greenViewport.RenderTarget);
        scene.BlueCamera.RenderTargets.Add(blueViewport.RenderTarget);

        var sideViews = new FlexBox() { Style = new() { Stack = StackKind.Vertical } };

        this.canvas.AppendChild(root);
        this.AppendChild(scene);

            root.AppendChild(verticalStack);

                verticalStack.AppendChild(header);
                    header.AppendChild(new FrameStatus());

                verticalStack.AppendChild(content);
                    content.AppendChild(viewports);

                        viewports.AppendChild(freeViewport);
                        viewports.AppendChild(sideViews);

                            sideViews.AppendChild(redViewport);
                            sideViews.AppendChild(greenViewport);
                            sideViews.AppendChild(blueViewport);
    }

    private void HandleBorders()
    {
        var reload = true;

        if (Input.IsKeyJustPressed(Key.Num1))
        {
            this.setup = AlignmentTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num2))
        {
            this.setup = BaselineTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num3))
        {
            this.setup = BoxModelTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num4))
        {
            this.setup = BoxSizingTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num5))
        {
            this.setup = ContentJustificationTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num6))
        {
            this.setup = ClippingTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num7))
        {
            this.setup = MarginTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num8))
        {
            this.setup = PaddingTest.Setup;
        }
        else if (Input.IsKeyJustPressed(Key.Num9))
        {
            this.setup = Playground.Setup;
        }
        else
        {
            reload = Input.IsKeyJustPressed(Key.R);
        }

        if (reload)
        {
            this.Reload();
        }
    }

    private void Reload()
    {
        this.canvas.DisposeChildren();

        this.setup.Invoke(this.canvas);
    }

#if DEBUG
    protected override void Connected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated += this.Reload;

    protected override void Disconnected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated -= this.Reload;
#endif

    public override void Update() =>
        this.HandleBorders();
}
