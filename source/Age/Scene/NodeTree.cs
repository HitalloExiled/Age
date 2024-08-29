using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scene;

public sealed class NodeTree
{
    public record struct Command2DEntry(Matrix3x2<float> Transform, Command Command);
    public record struct Command3DEntry(Matrix4x4<float> Transform, Command Command);

    private readonly List<Command2DEntry> command2DEntriesCache = [];
    private readonly List<Command3DEntry> command3DEntriesCache = [];
    private readonly Window window;

    private Buffer                     buffer = null!;
    private CanvasIndexRenderGraphPass canvasIndexRenderGraphPass = null!;
    private Element?                   lastFocusedElement;
    private Element?                   lastSelectedElement;

    internal List<Node>    Nodes    { get; } = [];
    internal List<Scene3D> Scenes3D { get; } = [];

    public bool   IsDirty { get; internal set; }
    public Root   Root    { get; } = new();
    public Window Window  => this.window;

    public NodeTree(Window window)
    {
        this.window = window;
        this.Root   = new() { Tree = this };

        this.Window.Closed += this.Destroy;
    }

    private static MouseEvent CreateEventTarget(Element target, in Platforms.Display.MouseEvent mouseEvent) =>
        new()
        {
            Target    = target,
            Button    = mouseEvent.Button,
            Delta     = mouseEvent.Delta,
            KeyStates = mouseEvent.KeyStates,
            X         = mouseEvent.X,
            Y         = mouseEvent.Y,
        };

    private Element? GetElement(ushort x, ushort y) =>
        this.GetNode(x, y) switch
        {
            Element  element  => element,
            TextNode textNode => textNode.ParentElement,
            _ => null,
        };

    private unsafe Node? GetNode(ushort x, ushort y)
    {
        var image = this.canvasIndexRenderGraphPass.ColorImage;

        if (x < image.Extent.Width && y < image.Extent.Height)
        {
            image.CopyToBuffer(this.buffer);

            this.buffer.Allocation.Memory.Map(0, (uint)this.buffer.Allocation.Size, 0, out var imageIndexBuffer);

            var imageIndex = new Span<uint>((uint*)imageIndexBuffer, (int)this.buffer.Allocation.Size / sizeof(uint));

            var index = x + y * image.Extent.Width;
            var pixel = imageIndex[(int)index];

            var id = (int)(pixel & 0x0000FFFF) - 1;

            this.buffer.Allocation.Memory.Unmap();

            // Console.WriteLine($"Element Index: {id}, Pixel Index: {index}, Color: {(Color)pixel}, Mouse Position: [{x}, {y}]");

            if (id > -1 && id < this.Nodes.Count)
            {
                return this.Nodes[id];
            }
        }

        return null;
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(uint);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    private void OnContextMenu(in Platforms.Display.ContextEvent contextEvent)
    {
        var element = this.GetElement(contextEvent.X, contextEvent.Y);

        if (element != null)
        {
            var eventTarget = new ContextEvent
            {
                X       = contextEvent.X,
                Y       = contextEvent.Y,
                ScreenX = contextEvent.ScreenX,
                ScreenY = contextEvent.ScreenY,
                Target  = element
            };

            element.InvokeContext(eventTarget);
        }

    }

    private void OnMouseClick(in Platforms.Display.MouseEvent mouseEvent)
    {
        var element = this.GetElement(mouseEvent.X, mouseEvent.Y);

        if (element != null)
        {
            var eventTarget = CreateEventTarget(element, mouseEvent);

            element.InvokeClick(eventTarget);

            if (element != this.lastFocusedElement)
            {
                element.InvokeFocus(eventTarget);

                this.lastFocusedElement?.InvokeBlur(CreateEventTarget(this.lastFocusedElement, mouseEvent));
            }

            this.lastFocusedElement = element;
        }
        else
        {
            this.lastFocusedElement?.InvokeBlur(CreateEventTarget(this.lastFocusedElement, mouseEvent));
            this.lastFocusedElement = null;
        }
    }

    private unsafe void OnMouseMove(in Platforms.Display.MouseEvent mouseEvent)
    {
        var element = this.GetElement(mouseEvent.X, mouseEvent.Y);

        if (element != null)
        {
            var eventTarget = CreateEventTarget(element, mouseEvent);

            element.InvokeMouseMoved(eventTarget);

            if (element != this.lastSelectedElement)
            {
                element.InvokeMouseOver(eventTarget);

                this.lastSelectedElement?.InvokeMouseOut(CreateEventTarget(this.lastSelectedElement, mouseEvent));
            }

            this.lastSelectedElement = element;
        }
        else
        {
            this.lastSelectedElement?.InvokeMouseOut(CreateEventTarget(this.lastSelectedElement, mouseEvent));
            this.lastSelectedElement = null;
        }
    }

    internal IEnumerable<Command2DEntry> Enumerate2DCommands()
    {
        if (this.command2DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command2DEntriesCache.Count; i++)
            {
                yield return this.command2DEntriesCache[i];
            }
        }
        else
        {
            foreach (var node in this.Root.Traverse())
            {
                if (node is Node2D node2D)
                {
                    var transform = (Matrix3x2<float>)node2D.TransformCache;

                    foreach (var command in node2D.Commands)
                    {
                        var entry = new Command2DEntry(transform, command);

                        this.command2DEntriesCache.Add(entry);

                        yield return entry;
                    }
                }
            }
        }
    }

    internal IEnumerable<Command3DEntry> Enumerate3DCommands()
    {
        if (this.command3DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command3DEntriesCache.Count; i++)
            {
                yield return this.command3DEntriesCache[i];
            }
        }
        else
        {
            foreach (var node in this.Root.Traverse())
            {
                if (node is Node3D node3D)
                {
                    var transform = (Matrix4x4<float>)node3D.TransformCache;

                    foreach (var command in node3D.Commands)
                    {
                        var entry = new Command3DEntry(transform, command);

                        this.command3DEntriesCache.Add(entry);

                        yield return entry;
                    }
                }
            }
        }
    }

    internal void ResetCache()
    {
        this.command2DEntriesCache.Clear();
        this.command3DEntriesCache.Clear();
    }

    public void Destroy()
    {
        this.window.Click     -= this.OnMouseClick;
        this.window.Context   -= this.OnContextMenu;
        this.window.MouseMove -= this.OnMouseMove;

        this.Root.Destroy();
        this.buffer.Dispose();
    }

    internal void Initialize()
    {
        this.canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasIndexRenderGraphPass>();
        this.canvasIndexRenderGraphPass.Recreated += this.UpdateBuffer;

        this.UpdateBuffer();

        this.window.Click     += this.OnMouseClick;
        this.window.Context   += this.OnContextMenu;
        this.window.MouseMove += this.OnMouseMove;

        foreach (var node in this.Root.Traverse())
        {
            node.Initialize();
        }

        foreach (var node in this.Root.Traverse())
        {
            node.LateUpdate();
        }
    }

    public void Update(double deltaTime)
    {
        foreach (var node in this.Root.Traverse())
        {
            node.Update(deltaTime);
        }

        foreach (var node in this.Root.Traverse())
        {
            node.LateUpdate();
        }
    }
}
