using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Image : Resource<VkImage>
{
    [MemberNotNullWhen(false, nameof(Allocation))]
    public bool IsBorrowed { get; private set; }

    public Allocation? Allocation { get; init; }

    public required VkExtent3D Extent { get; init; }

    internal Image(VulkanRenderer renderer, VkImage image) : base(renderer, image) { }

    internal static Image From(VulkanRenderer renderer, VkImage image, VkExtent3D extent) =>
        new(renderer, image)
        {
            Extent     = extent,
            IsBorrowed = true,
        };

    protected override void OnDispose()
    {
        if (!this.IsBorrowed)
        {
            this.Allocation.Dispose();
            this.Value.Dispose();
        }
    }

    public unsafe uint[] ReadBuffer(VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var size = this.Extent.Width * this.Extent.Height * sizeof(uint);

        using var buffer = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        this.CopyToBuffer(buffer, aspectMask);

        buffer.Allocation.Memory.Map(0, size, 0, out var data);

        var pixels = new Span<uint>((uint*)data, (int)(size / 4)).ToArray();

        return pixels;
    }

    public void TransitionImageLayout(
        VkImageLayout        oldLayout,
        VkImageLayout        newLayout,
        VkAccessFlags        srcAccessMask,
        VkAccessFlags        dstAccessMask,
        VkPipelineStageFlags sourceStage,
        VkPipelineStageFlags destinationStage
    )
    {
        var commandBuffer = this.Renderer.BeginSingleTimeCommands();

        var imageMemoryBarrier = new VkImageMemoryBarrier
        {
            DstAccessMask       = dstAccessMask,
            DstQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            Image               = this.Value.Handle,
            NewLayout           = newLayout,
            OldLayout           = oldLayout,
            SrcAccessMask       = srcAccessMask,
            SrcQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            SubresourceRange    = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
                LevelCount = 1,
            }
        };

        commandBuffer.Value.PipelineBarrier(sourceStage, destinationStage, default, [], [], [imageMemoryBarrier]);

        this.Renderer.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyFromBuffer(Buffer buffer)
    {
        var commandBuffer = this.Renderer.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent      = this.Extent,
            ImageSubresource = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
            }
        };

        commandBuffer.Value.CopyBufferToImage(buffer, this, VkImageLayout.TransferDstOptimal, [bufferImageCopy]);

        this.Renderer.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyToBuffer(Buffer buffer, VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var commandBuffer = this.Renderer.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent      = this.Extent,
            ImageSubresource = new()
            {
                AspectMask = aspectMask,
                LayerCount = 1,
            }
        };

        commandBuffer.Value.CopyImageToBuffer(this, VkImageLayout.TransferSrcOptimal, buffer.Value, [bufferImageCopy]);

        this.Renderer.EndSingleTimeCommands(commandBuffer);
    }

    public void Update(Span<byte> data)
    {
        var buffer = this.Renderer.CreateBuffer((ulong)data.Length, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        buffer.Allocation.Memory.Write(0, 0, data);

        this.TransitionImageLayout(
            VkImageLayout.Undefined,
            VkImageLayout.TransferDstOptimal,
            default,
            VkAccessFlags.TransferWrite,
            VkPipelineStageFlags.TopOfPipe,
            VkPipelineStageFlags.Transfer
        );

        this.CopyFromBuffer(buffer);

        this.TransitionImageLayout(
            VkImageLayout.TransferDstOptimal,
            VkImageLayout.ShaderReadOnlyOptimal,
            VkAccessFlags.TransferWrite,
            VkAccessFlags.ShaderRead,
            VkPipelineStageFlags.Transfer,
            VkPipelineStageFlags.FragmentShader
        );

        buffer.Dispose();
    }
}
