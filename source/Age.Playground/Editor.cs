using Age.Playground.Tests;
using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;
using Age.RenderPasses;
using Age.Scene;
using Age.Styling;
using ThirdParty.Vulkan.Flags;

using Common = Age.Internal.Common;

namespace Age.Playground;

internal delegate void Setup(Canvas canvas);

public class Editor : Node
{
    private const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();

    private Setup setup;

    public override string NodeName => nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.setup = ShadowTreeTest.Setup;

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
                StackDirection = StackDirection.Vertical,
                Size           = new((Percentage)100),
                Border         = new(BORDER_SIZE, default, Color.Yellow),
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
                Alignment = Alignment.Center,
                Border    = new(BORDER_SIZE, default, Color.Blue),
            }
        };

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

        var sideViews = new FlexBox() { Style = new() { StackDirection = StackDirection.Vertical } };

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

    private void HandleInputs()
    {
        var reload = true;

        var window = ((RenderTree)this.canvas.Tree!).Window;

        if (Input.IsKeyJustPressed(Key.Num1))
        {
            this.setup = ShadowTreeTest.Setup;

            window.Title = $"1 - {nameof(ShadowTreeTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num2))
        {
            this.setup = BaselineTest.Setup;

            window.Title = $"2 - {nameof(BaselineTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num3))
        {
            this.setup = BoxModelTest.Setup;

            window.Title = $"3 - {nameof(BoxModelTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num4))
        {
            this.setup = BoxSizingTest.Setup;

            window.Title = $"4 - {nameof(BoxSizingTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num5))
        {
            this.setup = ContentJustificationTest.Setup;

            window.Title = $"5 - {nameof(ContentJustificationTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num6))
        {
            this.setup = ClippingTest.Setup;

            window.Title = $"6 - {nameof(ClippingTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num7))
        {
            this.setup = MarginTest.Setup;

            window.Title = $"7 - {nameof(MarginTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num8))
        {
            this.setup = PaddingTest.Setup;

            window.Title = $"8 - {nameof(PaddingTest)}";
        }
        else if (Input.IsKeyJustPressed(Key.Num9))
        {
            this.setup = Tests.Playground.Setup;

            window.Title = $"9 - {nameof(Playground)}";
        }
        else if (Input.IsKeyPressed(Key.Control) && Input.IsKeyJustPressed(Key.P))
        {
            PrintCanvasIndex();
            reload = false;
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

    private static void PrintCanvasIndex()
    {
        var canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasIndexRenderGraphPass>();

        var canvasIndexImage = canvasIndexRenderGraphPass.ColorImage;

        Common.SaveImage(canvasIndexImage, VkImageAspectFlags.Color, "CanvasIndex.png");
    }

    private void Reload()
    {
        this.canvas.DisposeChildren();

        this.setup.Invoke(this.canvas);
    }

#if DEBUG
    protected override void OnConnected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated += this.Reload;

    protected override void OnDisconnected(NodeTree tree) =>
        HotReloadService.ApplicationUpdated -= this.Reload;
#endif

    public override void Update() =>
        this.HandleInputs();
}
