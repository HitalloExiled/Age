using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.Elements;
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

    private Buffer                     buffer = null!;
    private ulong                      bufferVersion;
    private CanvasIndexRenderGraphPass canvasIndexRenderGraphPass = null!;

    internal List<Node> Nodes { get; } = new(256);

    public Window Window { get; }

    public RenderTree(Window window)
    {
        this.Window = window;

        this.Window.Closed += this.Dispose;

        this.Window.Connect();
    }

    private void InitializeTree()
    {
        var enumerator = this.Window.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.IsUpdatesSuspended)
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Initialize();
            }
        }
    }

    private void LateUpdateTree()
    {
        var enumerator = this.Window.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.IsUpdatesSuspended)
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.LateUpdate();
            }
        }
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(ulong);

        this.buffer = new Buffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    private void UpdateTimers()
    {
        foreach (var timer in this.Timers)
        {
            timer.Update();
        }
    }

    private void UpdateTree()
    {
        var enumerator = this.Window.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.IsUpdatesSuspended)
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Update();

                if (current.IsChildrenUpdatesSuspended)
                {
                    enumerator.SkipToNextSibling();
                }
            }
        }
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

    public void Initialize()
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

        this.InitializeTree();
        this.LateUpdateTree();
    }

    public void Update()
    {
        if (this.framesUntilRequest > 0 && --this.framesUntilRequest == 0)
        {
            this.InvokeMouseRequestedEvent();
        }

        this.UpdateTimers();
        this.UpdateTree();
        this.LateUpdateTree();

        this.Updated?.Invoke();

        while (this.updatesQueue.Count > 0)
        {
            this.updatesQueue.Dequeue().Invoke();
        }

        if (this.IsDirty)
        {
            this.BuildIndexAndCollectCommands();
        }
    }
}
