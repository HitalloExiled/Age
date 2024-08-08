using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Image : Resource<VkImage>
{
    [MemberNotNullWhen(true, nameof(Allocation))]
    private bool OwnsHandler { get; set; } = true;

    public Allocation? Allocation { get; init; }

    public required VkExtent3D        Extent { get; init; }
    public required VkFormat          Format { get; init; }
    public required VkImageType       Type   { get; init; }
    public required VkImageUsageFlags Usage  { get; init; }

    public bool Initialized { get; internal set; }

    public int BytesPerPixel =>
        this.Format switch
        {
            VkFormat.R4G4UnormPack8
            or VkFormat.R8Unorm
            or VkFormat.R8Snorm
            or VkFormat.R8Uscaled
            or VkFormat.R8Sscaled
            or VkFormat.R8Uint
            or VkFormat.R8Sint
            or VkFormat.R8Srgb => 1,
            VkFormat.R4G4B4A4UnormPack16
            or VkFormat.B4G4R4A4UnormPack16
            or VkFormat.R5G6B5UnormPack16
            or VkFormat.B5G6R5UnormPack16
            or VkFormat.R5G5B5A1UnormPack16
            or VkFormat.B5G5R5A1UnormPack16
            or VkFormat.A1R5G5B5UnormPack16
            or VkFormat.R8G8Unorm
            or VkFormat.R8G8Snorm
            or VkFormat.R8G8Uscaled
            or VkFormat.R8G8Sscaled
            or VkFormat.R8G8Uint
            or VkFormat.R8G8Sint
            or VkFormat.R8G8Srgb
            or VkFormat.R16Unorm
            or VkFormat.R16Snorm
            or VkFormat.R16Uscaled
            or VkFormat.R16Sscaled
            or VkFormat.R16Uint
            or VkFormat.R16Sint
            or VkFormat.R16Sfloat => 2,
            VkFormat.R8G8B8Unorm
            or VkFormat.R8G8B8Snorm
            or VkFormat.R8G8B8Uscaled
            or VkFormat.R8G8B8Sscaled
            or VkFormat.R8G8B8Uint
            or VkFormat.R8G8B8Sint
            or VkFormat.R8G8B8Srgb
            or VkFormat.B8G8R8Unorm
            or VkFormat.B8G8R8Snorm
            or VkFormat.B8G8R8Uscaled
            or VkFormat.B8G8R8Sscaled
            or VkFormat.B8G8R8Uint
            or VkFormat.B8G8R8Sint
            or VkFormat.B8G8R8Srgb => 3,
            VkFormat.R8G8B8A8Unorm
            or VkFormat.R8G8B8A8Snorm
            or VkFormat.R8G8B8A8Uscaled
            or VkFormat.R8G8B8A8Sscaled
            or VkFormat.R8G8B8A8Uint
            or VkFormat.R8G8B8A8Sint
            or VkFormat.R8G8B8A8Srgb
            or VkFormat.B8G8R8A8Unorm
            or VkFormat.B8G8R8A8Snorm
            or VkFormat.B8G8R8A8Uscaled
            or VkFormat.B8G8R8A8Sscaled
            or VkFormat.B8G8R8A8Uint
            or VkFormat.B8G8R8A8Sint
            or VkFormat.B8G8R8A8Srgb
            or VkFormat.A8B8G8R8UnormPack32
            or VkFormat.A8B8G8R8SnormPack32
            or VkFormat.A8B8G8R8UscaledPack32
            or VkFormat.A8B8G8R8SscaledPack32
            or VkFormat.A8B8G8R8UintPack32
            or VkFormat.A8B8G8R8SintPack32
            or VkFormat.A8B8G8R8SrgbPack32
            or VkFormat.A2R10G10B10UnormPack32
            or VkFormat.A2R10G10B10SnormPack32
            or VkFormat.A2R10G10B10UscaledPack32
            or VkFormat.A2R10G10B10SscaledPack32
            or VkFormat.A2R10G10B10UintPack32
            or VkFormat.A2R10G10B10SintPack32
            or VkFormat.A2B10G10R10UnormPack32
            or VkFormat.A2B10G10R10SnormPack32
            or VkFormat.A2B10G10R10UscaledPack32
            or VkFormat.A2B10G10R10SscaledPack32
            or VkFormat.A2B10G10R10UintPack32
            or VkFormat.A2B10G10R10SintPack32
            or VkFormat.R16G16Unorm
            or VkFormat.R16G16Snorm
            or VkFormat.R16G16Uscaled
            or VkFormat.R16G16Sscaled
            or VkFormat.R16G16Uint
            or VkFormat.R16G16Sint
            or VkFormat.R16G16Sfloat
            or VkFormat.R32Uint
            or VkFormat.R32Sint
            or VkFormat.R32Sfloat => 4,
            VkFormat.R16G16B16Unorm
            or VkFormat.R16G16B16Snorm
            or VkFormat.R16G16B16Uscaled
            or VkFormat.R16G16B16Sscaled
            or VkFormat.R16G16B16Uint
            or VkFormat.R16G16B16Sint
            or VkFormat.R16G16B16Sfloat => 6,
            VkFormat.R16G16B16A16Unorm
            or VkFormat.R16G16B16A16Snorm
            or VkFormat.R16G16B16A16Uscaled
            or VkFormat.R16G16B16A16Sscaled
            or VkFormat.R16G16B16A16Uint
            or VkFormat.R16G16B16A16Sint
            or VkFormat.R16G16B16A16Sfloat
            or VkFormat.R32G32Uint
            or VkFormat.R32G32Sint
            or VkFormat.R32G32Sfloat
            or VkFormat.R64Uint
            or VkFormat.R64Sint
            or VkFormat.R64Sfloat => 8,
            VkFormat.R32G32B32Uint
            or VkFormat.R32G32B32Sint
            or VkFormat.R32G32B32Sfloat => 12,
            VkFormat.R32G32B32A32Uint
            or VkFormat.R32G32B32A32Sint
            or VkFormat.R32G32B32A32Sfloat
            or VkFormat.R64G64Uint
            or VkFormat.R64G64Sint
            or VkFormat.R64G64Sfloat => 16,
            VkFormat.R64G64B64Uint
            or VkFormat.R64G64B64Sint
            or VkFormat.R64G64B64Sfloat => 24,
            VkFormat.R64G64B64A64Uint
            or VkFormat.R64G64B64A64Sint
            or VkFormat.R64G64B64A64Sfloat => 32,
            _ => 0,
        };

    internal Image(VkImage image) : base(image) { }

    internal static Image From(VkImage image, VkExtent3D extent, VkFormat format, VkImageType type, VkImageUsageFlags usage) =>
        new(image)
        {
            Extent      = extent,
            Format      = format,
            Type        = type,
            OwnsHandler = false,
            Usage       = usage,
        };

    protected override void OnDispose()
    {
        if (this.OwnsHandler)
        {
            this.Allocation.Dispose();
            this.Value.Dispose();
        }
    }

    public void CopyFromBuffer(Buffer buffer)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

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

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyToBuffer(Buffer buffer, VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

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

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public unsafe uint[] ReadBuffer(VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var size = this.Extent.Width * this.Extent.Height;

        using var buffer = VulkanRenderer.Singleton.CreateBuffer(size * sizeof(uint), VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        this.CopyToBuffer(buffer, aspectMask);

        buffer.Allocation.Memory.Map(0, size, 0, out var data);

        var pixels = new Span<uint>((uint*)data, (int)size).ToArray();

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
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

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

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void Update(Span<byte> data)
    {
        var buffer = VulkanRenderer.Singleton.CreateBuffer((ulong)data.Length, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

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
