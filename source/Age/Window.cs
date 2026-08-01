using Age.Core.Extensions;
using Age.Core;
using Age.Graphs;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Scenes;

using DisplayWindow           = Age.Platforms.Display.Window;
using WindowMouseEventHandler = Age.Platforms.Display.WindowMouseEventHandler;

namespace Age;

public sealed class Window : Viewport
{
    public event WindowMouseEventHandler? Click
    {
        add    => this.window.Click += value;
        remove => this.window.Click -= value;
    }

    public event Action? Closed
    {
        add    => this.window.Closed += value;
        remove => this.window.Closed -= value;
    }

    public event WindowContextEventHandler? Context
    {
        add    => this.window.Context += value;
        remove => this.window.Context -= value;
    }

    public event WindowMouseEventHandler? DoubleClick
    {
        add    => this.window.DoubleClick += value;
        remove => this.window.DoubleClick -= value;
    }

    public event WindowInputEventHandler? Input
    {
        add    => this.window.Input += value;
        remove => this.window.Input -= value;
    }

    public event WindowKeyEventHandler? KeyDown
    {
        add    => this.window.KeyDown += value;
        remove => this.window.KeyDown -= value;
    }

    public event WindowKeyEventHandler? KeyPress
    {
        add    => this.window.KeyPress += value;
        remove => this.window.KeyPress -= value;
    }

    public event WindowKeyEventHandler? KeyUp
    {
        add    => this.window.KeyUp += value;
        remove => this.window.KeyUp -= value;
    }

    public event WindowMouseEventHandler? MouseDown
    {
        add    => this.window.MouseDown += value;
        remove => this.window.MouseDown -= value;
    }

    public event WindowMouseEventHandler? MouseMove
    {
        add    => this.window.MouseMove += value;
        remove => this.window.MouseMove -= value;
    }

    public event WindowMouseEventHandler? MouseUp
    {
        add    => this.window.MouseUp += value;
        remove => this.window.MouseUp -= value;
    }

    public event WindowMouseEventHandler? MouseWheel
    {
        add    => this.window.MouseWheel += value;
        remove => this.window.MouseWheel -= value;
    }

    public override event Action? Resized
    {
        add    => this.window.Resized += value;
        remove => this.window.Resized -= value;
    }

    private static readonly List<Window> windows = [];

    private readonly RenderTarget[] renderTargets;
    private readonly DisplayWindow  window;

    public static Cursor Cursor
    {
        get => DisplayWindow.Cursor;
        set => DisplayWindow.Cursor = value;
    }

    public static ReadOnlySpan<Window> Windows => windows.AsSpan();

    public Surface Surface { get; }

    public override Size<uint> Size
    {
        get => this.window.Size;
        set => Logger.Warn("Window size cant be modified");
    }

    public string Title
    {
        get => this.window.Title;
        set => this.window.Title = value;
    }

    public RenderTree RenderTree { get; }

    public override RenderGraph RenderGraph { get; }

    public bool IsClosed    => this.window.IsClosed;
    public bool IsMaximized => this.window.IsMaximized;
    public bool IsMinimized => this.window.IsMinimized;
    public bool IsVisible   => this.window.IsVisible;

    public override bool         IsDirty      => this.Surface.IsDirty;
    public override string       NodeName     => nameof(Age.Window);
    public override RenderTarget RenderTarget => this.renderTargets[this.Surface.CurrentBuffer];
    public override Texture2D    Texture      => Texture2D.Empty;

    public Window(string title, in Size<uint> size, Window? parent = null)
    {
        this.MakeSubtreeStatePristine();

        this.window = new DisplayWindow(title, size, parent?.window);

#if WINDOWS
        this.Surface = VulkanRenderer.Singleton.CreateSurface(this.window.Handle, this.window.Size);
#else
        this.Surface = VulkanRenderer.Singleton.CreateSurface(WindowManager.Instance.Display, this.window.Surface, this.window.Size);
#endif

        this.renderTargets = new RenderTarget[this.Surface.Swapchain.Images.Length];

        this.Surface.SwapchainRecreated += this.CreateRenderTargets;
        this.CreateRenderTargets();

        this.window.Resized += this.OnWindowResized;
        this.window.Closed  += this.Dispose;

        windows.Add(this);

        this.RenderGraph = RenderGraph.CreateDefault(this);
        this.RenderTree  = new RenderTree(this);

        this.Connect();
    }

    private static RenderTarget CreateRenderTarget(Image image)
    {
        var createInfo = new RenderTarget.MultiPassCreateInfo
        {
            Size = new(image.Extent.Width, image.Extent.Height),
            Attachments =
            [
                RenderTarget.CreateInfo.ColorAttachmentInfo.From(image, ImageLayout.PresentSrcKHR),
                new RenderTarget.CreateInfo.DepthStencilAttachmentInfo
                {
                    FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
                    Format      = VulkanRenderer.Singleton.StencilBufferFormat,
                    Aspect      = TextureAspect.Stencil,
                }
            ],
            Passes =
            [
                new()
                {
                    ColorAttachments       = [0],
                    DepthStencilAttachment = 1
                },
                new()
                {
                    ColorAttachments       = [0],
                    DepthStencilAttachment = 1
                }
            ],
            Dependencies =
            [
                new()
                {
                    SrcSubpass    = RenderTarget.SUBPASS_EXTERNAL,
                    DstSubpass    = 0,
                    SrcStageMask  = PipelineStage.ColorAttachmentOutput,
                    DstStageMask  = PipelineStage.ColorAttachmentOutput,
                    SrcAccessMask = AccessMask.ColorAttachmentWrite,
                    DstAccessMask = AccessMask.ColorAttachmentRead | AccessMask.ColorAttachmentWrite,
                },
                new()
                {
                    SrcSubpass    = RenderTarget.SUBPASS_EXTERNAL,
                    DstSubpass    = 1,
                    SrcStageMask  = PipelineStage.ColorAttachmentOutput,
                    DstStageMask  = PipelineStage.ColorAttachmentOutput,
                    SrcAccessMask = AccessMask.ColorAttachmentWrite,
                    DstAccessMask = AccessMask.ColorAttachmentRead | AccessMask.ColorAttachmentWrite,
                }
            ]
        };

        return new(createInfo);
    }

    private void CreateRenderTargets()
    {
        for (var i = 0; i < this.Surface.Swapchain.Images.Length; i++)
        {
            this.renderTargets[i]?.Dispose();
            this.renderTargets[i] = CreateRenderTarget(this.Surface.Swapchain.Images[i]);
        }
    }

    private void OnWindowResized()
    {
        if (this.Surface.Visible = this.window.IsVisible && !this.window.IsMinimized)
        {
            this.Surface.Size = this.window.Size;
        }
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        windows.Remove(this);

        foreach (var renderTarget in this.renderTargets)
        {
            renderTarget.Dispose();
        }

        VulkanRenderer.Singleton.DeferredDispose(this.Surface);

        this.RenderTree.Dispose();
        this.RenderGraph.Dispose();
        this.window.Close();
    }

    public void Close() =>
        this.Dispose();

    public void DoEvents() =>
        this.window.DoEvents();

    public string? GetClipboardData() =>
        this.window.GetClipboardData();

    public void Hide() =>
        this.window.Hide();

    public void Minimize() =>
        this.window.Minimize();

    public void Maximize() =>
        this.window.Maximize();

    public void Restore() =>
        this.window.Restore();

    public void SetClipboardData(string value) =>
        this.window.SetClipboardData(value);

    public void Show() =>
        this.window.Show();
}
