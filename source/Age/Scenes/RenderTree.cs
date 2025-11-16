using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.RenderPasses;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scenes;

public sealed partial class RenderTree : Disposable
{
    public event Action? Updated;

    private readonly Queue<Action> updatesQueue = [];

    internal List<Timer> Timers { get; } = [];

    public bool IsDirty { get; private set; }

    [AllowNull]
    private Buffer buffer;

    private ulong bufferVersion;

    [AllowNull]
    private CanvasEncodeRenderGraphPass canvasIndexRenderGraphPass;

    public Window Window { get; }

    public RenderTree(Window window)
    {
        window.Closed += this.Dispose;

        window.Connect();

        this.Window = window;
        this.viewports.Add(window);
    }

    private void ExecuteLateUpdates()
    {
        for (var i = 0; i < this.Nodes.Count; i++)
        {
            var node = this.Nodes[i];

            if (!node.IsUpdatesSuspended)
            {
                node.LateUpdate();
            }

            if (node.IsChildrenUpdatesSuspended)
            {
                i = node.SubtreeRange.End - 1;
            }
        }
    }

    private void ExecuteUpdates()
    {
        for (var i = 0; i < this.Nodes.Count; i++)
        {
            var node = this.Nodes[i];

            node.InvokeStart();

            if (!node.IsUpdatesSuspended)
            {
                node.Update();
            }

            if (node.IsChildrenUpdatesSuspended)
            {
                i = node.SubtreeRange.End - 1;
            }
        }
    }

    private void ExecuteTimersUpdate()
    {
        foreach (var timer in this.Timers)
        {
            timer.Update();
        }
    }

    [MemberNotNull(nameof(buffer))]
    private void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(ulong);

        this.buffer = new Buffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
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

            this.buffer.Dispose();
        }
    }

    internal void AddDeferredUpdate(Action action)
    {
        this.updatesQueue.Enqueue(action);
        this.MakeDirty();
    }

    internal void MakePristine() =>
        this.IsDirty = false;

    internal void MakeDirty() =>
        this.IsDirty = true;

    internal ReadOnlySpan<Command> Get2DCommands() =>
        this.Window.RenderContext.Buffer2D.Commands;

    internal ReadOnlySpan<Command> Get3DCommands() =>
        this.Window.RenderContext.Buffer3D.Commands;

    internal ReadOnlySpan<Viewport> GetViewports() =>
        this.viewports.AsSpan();

    public void Initialize()
    {
        this.canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasEncodeRenderGraphPass>();
        this.canvasIndexRenderGraphPass.Recreated += this.UpdateBuffer;

        this.UpdateBuffer();

        this.Window.Context     += this.OnContext;
        this.Window.DoubleClick += this.OnDoubleClick;
        this.Window.KeyDown     += this.OnKeyDown;
        this.Window.MouseDown   += this.OnMouseDown;
        this.Window.MouseMove   += this.OnMouseMove;
        this.Window.MouseUp     += this.OnMouseUp;
        this.Window.MouseWheel  += this.OnMouseWheel;

        this.BuildSceneGraphCache();
    }

    public void Update()
    {
        if (this.framesUntilRequest > 0 && --this.framesUntilRequest == 0)
        {
            this.InvokeMouseRequestedEvent();
        }

        this.ExecuteTimersUpdate();
        this.ExecuteUpdates();
        this.ExecuteLateUpdates();

        this.Updated?.Invoke();

        while (this.updatesQueue.Count > 0)
        {
            this.updatesQueue.Dequeue().Invoke();
        }

        this.BuildSceneGraphCache();
    }
}
