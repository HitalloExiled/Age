using Age.Core.Extensions;
using Age.Elements;
using Age.Platforms.Display;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scene;

public sealed partial class RenderTree : NodeTree
{
    private readonly Stack<(Slot, int)> composedTreeStack = [];

    private Buffer                     buffer = null!;
    private ulong                      bufferVersion;
    private CanvasIndexRenderGraphPass canvasIndexRenderGraphPass = null!;

    internal List<Node>    Nodes    { get; } = new(256);
    internal List<Scene3D> Scenes3D { get; } = [];

    public Window Window { get; }

    public RenderTree(Window window)
    {
        this.Window = window;

        this.Window.Closed += this.Dispose;
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(ulong);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Window.Context     -= this.OnContext;
            this.Window.DoubleClick -= this.OnDoubleClick;
            this.Window.KeyDown     -= this.OnKeyDown;
            this.Window.MouseDown   -= this.OnMouseDown;
            this.Window.MouseMove   -= this.OnMouseMove;
            this.Window.MouseUp     -= this.OnMouseUp;
            this.Window.MouseWheel  -= this.OnMouseWheel;

            this.Root.Dispose();
            this.buffer.Dispose();
        }
    }

    internal ReadOnlySpan<Command2DEntry> Get2DCommands() =>
        this.command2DEntries.AsSpan();

    internal ReadOnlySpan<Command3DEntry> Get3DCommands() =>
        this.command3DEntries.AsSpan();

    public override void Initialize()
    {
        this.canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasIndexRenderGraphPass>();
        this.canvasIndexRenderGraphPass.Recreated += this.UpdateBuffer;

        this.UpdateBuffer();

        this.Window.Context     += this.OnContext;
        this.Window.DoubleClick += this.OnDoubleClick;
        this.Window.KeyDown     += this.OnKeyDown;
        this.Window.MouseDown   += this.OnMouseDown;
        this.Window.MouseMove   += this.OnMouseMove;
        this.Window.MouseUp     += this.OnMouseUp;
        this.Window.MouseWheel  += this.OnMouseWheel;

        base.Initialize();
    }

    public override void Update()
    {
        if (this.framesUntilRequest > 0 && --this.framesUntilRequest == 0)
        {
            this.InvokeMouseRequestedEvent();
        }
         
        base.Update();

        if (this.IsDirty)
        {
            this.ResetCache();
            this.BuildIndexAndCollectCommands();
        }
    }
}
