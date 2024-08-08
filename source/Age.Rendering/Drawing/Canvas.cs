using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Rendering.RenderPasses;
using Age.Rendering.Scene;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Drawing;

public sealed class Canvas : Element
{
    private const ushort PADDING = 8;

    public override string NodeName { get; } = nameof(Canvas);
    private readonly CanvasIndexRenderGraphPass canvasIndexRenderGraphPass;

    private Buffer buffer;

    public Canvas()
    {
        this.Style = new()
        {
            Baseline = 1,
            Position = new(PADDING, -PADDING),
        };

        this.canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasIndexRenderGraphPass>();

        this.canvasIndexRenderGraphPass.Recreated += this.UpdateBuffer;

        this.UpdateBuffer();
    }

    private void OnWindowSizeChanged() =>
        this.Style.Size = this.Tree!.Window.ClientSize - PADDING * 2;

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(uint);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    private unsafe void OnMouseMove(short x, short y)
    {
        var image = this.canvasIndexRenderGraphPass.ColorImage;

        if (this.IsConnected && x < image.Extent.Width && y < image.Extent.Height)
        {
            image.CopyToBuffer(this.buffer);

            this.buffer.Allocation.Memory.Map(0, (uint)this.buffer.Allocation.Size, 0, out var imageIndexBuffer);

            var imageIndex = new Span<uint>((uint*)imageIndexBuffer, (int)this.buffer.Allocation.Size / sizeof(uint));

            var index = x + y * image.Extent.Width;
            var pixel = imageIndex[(int)index];

            var id = (int)(pixel & 0x0000FFFF) - 1;

            this.buffer.Allocation.Memory.Unmap();

            Console.WriteLine($"Element Index: {id}, Pixel Index: {index}, Color: {(Color)pixel}, Mouse Position: [{x}, {y}]");

            if (id > -1 && id < this.Tree.Nodes.Count)
            {
                Console.WriteLine(this.Tree.Nodes[id].ToString());
            }
        }
    }

    internal protected override void RequestUpdate()
    {
        base.RequestUpdate();

        if (this.IsConnected)
        {
            this.Tree.IsDirty = true;
        }
    }

    protected override void OnConnected(NodeTree tree)
    {
        tree.Window.MouseMove   += this.OnMouseMove;
        tree.Window.SizeChanged += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
    }

    protected override void OnDisconnected(NodeTree tree)
    {
        tree.Window.MouseMove   -= this.OnMouseMove;
        tree.Window.SizeChanged -= this.OnWindowSizeChanged;
    }

    protected override void OnChildAppended(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = this;
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = null;
        }
    }

    protected override void OnDestroy()
    {
        this.canvasIndexRenderGraphPass.Recreated -= this.UpdateBuffer;
        this.buffer.Dispose();
    }

    public override void LateUpdate() =>
        this.UpdateLayout();
}
