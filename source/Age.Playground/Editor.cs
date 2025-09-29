using Age.Playground.Tests;
using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;
using Age.RenderPasses;
using Age.Scene;
using Age.Styling;
using ThirdParty.Vulkan.Flags;

using Common = Age.Internal.Common;
using Age.Playground.Tests.Styling;
using Age.Playground.Tests.Components;

namespace Age.Playground;

internal record struct Page(string Title, Setup Setup);

internal delegate void Setup(Canvas canvas);

public class Editor : Renderable
{
    private const uint BORDER_SIZE = 10;
    private readonly Canvas canvas = new();
    private readonly Page[] pages;
    private int index;

    private Setup setup;

    public override string NodeName => nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.setup = ScrollTest.Setup;

        this.pages =
        [
            new(nameof(ButtonTest),   ButtonTest.Setup),
            new(nameof(CheckBoxTest), CheckBoxTest.Setup),
            new(nameof(IconTest),     IconTest.Setup),
            new(nameof(TextBoxTest),  TextBoxTest.Setup),

            new(nameof(AlignmentTest),            AlignmentTest.Setup),
            new(nameof(BackgroundImageTest),      BackgroundImageTest.Setup),
            new(nameof(BaselineTest),             BaselineTest.Setup),
            new(nameof(BoxModelTest),             BoxModelTest.Setup),
            new(nameof(BoxSizingTest),            BoxSizingTest.Setup),
            new(nameof(ClippingTest),             ClippingTest.Setup),
            new(nameof(ContentJustificationTest), ContentJustificationTest.Setup),
            new(nameof(MarginTest),               MarginTest.Setup),
            new(nameof(PaddingTest),              PaddingTest.Setup),
            new(nameof(ScrollTest),               ScrollTest.Setup),

            new(nameof(ShadowTreeTest),    ShadowTreeTest.Setup),
            new(nameof(TextSelectionTest), TextSelectionTest.Setup),
        ];

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
                Size   = new(Unit.Pc(100)),
                Border = new(BORDER_SIZE, default, Color.Margenta),
            }
        };

        var verticalStack = new FlexBox()
        {
            Name  = "VStack",
            Style = new()
            {
                StackDirection = StackDirection.Vertical,
                Size           = new(Unit.Pc(100)),
                Border         = new(BORDER_SIZE, default, Color.Yellow),
            }
        };

        var header = new FlexBox
        {
            Name  = "Header",
            Style = new()
            {
                Size   = new(Unit.Pc(100), null),
                Border = new(BORDER_SIZE, default, Color.Red),
            }
        };

        var content = new FlexBox
        {
            Name  = "Content",
            Style = new()
            {
                Size   = new(Unit.Pc(100)),
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

        var freeViewport  = new SubViewport(new(600)) { Name = "Red" };
        var redViewport   = new SubViewport(new(200)) { Name = "Red" };
        var greenViewport = new SubViewport(new(200)) { Name = "Green" };
        var blueViewport  = new SubViewport(new(200)) { Name = "Blue" };

        // freeViewport.Style.Border  = new(1, 0, Color.White);
        // redViewport.Style.Border   = new(1, 0, Color.Red);
        // greenViewport.Style.Border = new(1, 0, Color.Green);
        // blueViewport.Style.Border  = new(1, 0, Color.Blue);

        // freeViewport.Style.BoxSizing = redViewport.Style.BoxSizing = greenViewport.Style.BoxSizing = blueViewport.Style.BoxSizing = BoxSizing.Border;

        freeViewport.Camera3D  = scene.FreeCamera;
        redViewport.Camera3D   = scene.RedCamera;
        greenViewport.Camera3D = scene.GreenCamera;
        blueViewport.Camera3D  = scene.BlueCamera;

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

        var window = this.canvas.Scene!.Viewport!.Window!;

        var currentIndex = this.index;

        if (Input.IsKeyJustPressed(Key.Up))
        {
            currentIndex = 0;
        }
        else if (Input.IsKeyJustPressed(Key.Down))
        {
            currentIndex = this.pages.Length - 1;
        }
        else if (Input.IsKeyJustPressed(Key.Left))
        {
            if (--currentIndex < 0)
            {
                currentIndex = this.pages.Length - 1;
            }
        }
        else if (Input.IsKeyJustPressed(Key.Right))
        {
            if (++currentIndex == this.pages.Length)
            {
                currentIndex = 0;
            }
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

        if (this.index != currentIndex)
        {
            var page = this.pages[currentIndex];

            window.Title = page.Title;

            this.setup = page.Setup;

            this.index = currentIndex;

            reload = true;
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
    protected override void OnConnected()
    {
        base.OnConnected();
        HotReloadService.ApplicationUpdated += this.Reload;
    }

    protected override void OnDisconnecting()
    {
        base.OnDisconnecting();
        HotReloadService.ApplicationUpdated -= this.Reload;
    }
#endif

    public override void Update() =>
        this.HandleInputs();
}
