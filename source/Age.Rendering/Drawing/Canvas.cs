using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Rendering.RenderPasses;
using Age.Rendering.Scene;
using Age.Rendering.Vulkan;
using SkiaSharp;
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

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(uint);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    private void OnWindowSizeChanged() =>
        this.Style.Size = this.Tree!.Window.ClientSize - PADDING * 2;

    private unsafe void OnMouseMove(short x, short y)
    {
        var image = this.canvasIndexRenderGraphPass.ColorImage;

        if (this.IsConnected && image.Initialized && x < image.Extent.Width && y < image.Extent.Height)
        {
            image.CopyToBuffer(this.buffer);

            this.buffer.Allocation.Memory.Map(0, (uint)this.buffer.Allocation.Size, 0, out var imageIndexBuffer);

            var imageIndex = new Span<uint>((uint*)imageIndexBuffer, (int)this.buffer.Allocation.Size / sizeof(uint));

            var index = x + y * image.Extent.Width;
            var pixel = imageIndex[(int)index];

            var id = pixel & 0x0000FFFF;

            this.buffer.Allocation.Memory.Unmap();

            Console.WriteLine($"Element Index: {id} at [{x}, {y}], pixel: {Convert.ToString(id, 16).PadLeft(8, '0')}: color: {(Color)pixel}, SkColor: {(SKColor)pixel}, {Convert.ToString(id, 2)}, position: {index}");

            if (id > 0 && id <= this.Tree.Nodes.Count)
            {
                Console.WriteLine(this.Tree.Nodes[(int)(id - 1)].ToString());
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

    protected override void OnConnected()
    {
        this.Tree!.Window.MouseMove   += this.OnMouseMove;
        this.Tree!.Window.SizeChanged += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
    }

    protected override void OnDisconnected(SceneTree tree)
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

    protected override void OnInitialize() =>
        this.UpdateLayout();

    protected override void OnPostUpdate(double deltaTime) =>
        this.UpdateLayout();
}
