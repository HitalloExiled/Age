using Age.Core.Extensions;
using Age.Core;
using Age.Passes;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scenes;

public sealed partial class RenderTree : Disposable
{
    public event Action? Updated;

    private readonly UISceneEncodePass uiSceneEncodePass;
    private readonly Queue<Action>     updatesQueue = [];

    private Buffer buffer;
    private ulong  bufferVersion;

    internal List<Timer> Timers { get; } = [];

    internal ReadOnlySpan<Viewport> Viewports => this.viewports.AsSpan();

    public bool IsDirty { get; private set; }

    public Window Window { get; }

    public RenderTree(Window window)
    {
        this.viewports.Add(window);

        window.Connect();

        window.Closed      += this.Dispose;
        window.Context     += this.OnContext;
        window.DoubleClick += this.OnDoubleClick;
        window.KeyDown     += this.OnKeyDown;
        window.MouseDown   += this.OnMouseDown;
        window.MouseMove   += this.OnMouseMove;
        window.MouseUp     += this.OnMouseUp;
        window.MouseWheel  += this.OnMouseWheel;
        window.Resized     += this.UpdateBuffer;

        this.uiSceneEncodePass = window.RenderGraph.GetNode<UISceneEncodePass>();

        this.UpdateBuffer();

        this.Window = window;
    }

    private void ExecuteLateUpdates()
    {
        var nodes = this.Nodes;

        for (var i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];

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
        var nodes = this.Nodes;

        for (var i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];

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

        var texture = this.uiSceneEncodePass.Output;

        var size = texture.Extent.Width * texture.Extent.Height * sizeof(ulong);

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

    public void Initialize() =>
        this.BuildSceneGraphCache();

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
