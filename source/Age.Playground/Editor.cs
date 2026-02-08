using Age.Core.Extensions;
using Age.Elements.Events;
using Age.Elements;
using Age.Platforms.Display;
using Age.Playground.Tests.Components;
using Age.Playground.Tests.Scene;
using Age.Playground.Tests.Styling;
using Age.Playground.Tests;
using Age.Scenes;
using System.Diagnostics;
using ThirdParty.Vulkan.Flags;
using Age.Internal;
using Age.Passes;

namespace Age.Playground;

internal record struct Page(string Title, Setup Setup);

internal delegate void Setup(Canvas canvas);

public sealed class Editor : UIScene
{
    private readonly Page[] pages;
    private int index;

    private Setup setup;

    public override string NodeName => nameof(Editor);

    public Editor()
    {
        this.setup = BaselineTest.Setup;

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

        this.Canvas.KeyDown += this.OnCanvasKeyDown;
    }

    private void OnCanvasKeyDown(in KeyEvent keyEvent)
    {
        var reload = true;

        var window = this.Scene!.Window!;

        var currentIndex = this.index;

        if (keyEvent.Key == Key.Up)
        {
            currentIndex = 0;
        }
        else if (keyEvent.Key == Key.Down)
        {
            currentIndex = this.pages.Length - 1;
        }
        else if (keyEvent.Key == Key.Left)
        {
            if (--currentIndex < 0)
            {
                currentIndex = this.pages.Length - 1;
            }
        }
        else if (keyEvent.Key == Key.Right)
        {
            if (++currentIndex == this.pages.Length)
            {
                currentIndex = 0;
            }
        }
        else if (keyEvent.Modifiers.HasFlags(KeyStates.Control) && keyEvent.Key == Key.P)
        {
            PrintCanvasIndex();
            reload = false;
        }
        else
        {
            reload = keyEvent.Key == Key.R;
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

    private void PrintCanvasIndex()
    {
        Debug.Assert(this.Scene?.Viewport != null);

        var scene2DEncodePass = this.Scene.Viewport.RenderGraph.GetNode<UISceneEncodePass>();

        Common.SaveImage(scene2DEncodePass.Output, VkImageAspectFlags.Color, "CanvasEncode.png");
    }

    private void Reload()
    {
        var start = Stopwatch.GetTimestamp();

        this.Canvas.DisposeChildren();

        this.setup.Invoke(this.Canvas);

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
}
