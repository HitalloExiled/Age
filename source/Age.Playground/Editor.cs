using Age.Playground.Tests;
using Age.Elements;
using Age.Platforms.Display;
using Age.RenderPasses;
using Age.Scenes;
using ThirdParty.Vulkan.Flags;

using Common = Age.Internal.Common;
using Age.Playground.Tests.Styling;
using Age.Playground.Tests.Components;
using System.Diagnostics;
using Age.Playground.Tests.Scene;

namespace Age.Playground;

internal record struct Page(string Title, Setup Setup);

internal delegate void Setup(Canvas canvas);

public class Editor : Scene2D
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
        this.setup = ShadowRootTest.Setup;

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
            new(nameof(ShadowRootTest),           ShadowRootTest.Setup),
            new(nameof(TextSelectionTest),        TextSelectionTest.Setup),

            new(nameof(SubViewportTest), SubViewportTest.Setup),

        ];
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
        var canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasEncodeRenderGraphPass>();

        var canvasIndexImage = canvasIndexRenderGraphPass.ColorImage;

        Common.SaveImage(canvasIndexImage, VkImageAspectFlags.Color, "CanvasIndex.png");
    }

    private void Reload()
    {
        var start = Stopwatch.GetTimestamp();

        this.canvas.DisposeChildren();

        this.setup.Invoke(this.canvas);

        Console.WriteLine($"Reload time: {Stopwatch.GetElapsedTime(start).TotalMilliseconds}ms");
    }

    protected override void OnStart()
    {
        base.OnStart();
        this.Reload();
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
