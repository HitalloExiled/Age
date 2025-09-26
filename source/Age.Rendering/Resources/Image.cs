using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using System.Numerics;
using Age.Core.Collections;

namespace Age.Rendering.Resources;

public sealed class Image : Resource<VkImage>
{
    private readonly Allocation? allocation;

    internal override VkImage Instance { get; }

    public VkExtent3D         Extent        { get; }
    public VkFormat           Format        { get; }
    public VkImageLayout      InitialLayout { get; }
    public VkSampleCountFlags Samples       { get; }
    public VkImageType        Type          { get; }
    public VkImageUsageFlags  Usage         { get; }

    public VkImageLayout FinalLayout { get; private set; }

    public VkImageAspectFlags Aspect
    {
        get
        {
            VkImageAspectFlags aspect = default;

            if (this.Usage.HasFlags(VkImageUsageFlags.ColorAttachment) || this.Usage.HasFlags(VkImageUsageFlags.Sampled))
            {
                aspect |= VkImageAspectFlags.Color;
            }

            if (this.Usage.HasFlags(VkImageUsageFlags.DepthStencilAttachment))
            {
                aspect |= VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil;
            }

            return aspect;
        }
    }

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

    internal Image(VkImage instance, in VkImageCreateInfo description)
    {
        this.Instance      = instance;
        this.Extent        = description.Extent;
        this.Format        = description.Format;
        this.Type          = description.ImageType;
        this.InitialLayout = description.InitialLayout;
        this.Samples       = description.Samples;
        this.Usage         = description.Usage;
    }

    public Image(in VkImageCreateInfo createInfo, VkImageLayout finalLayout = default)
    {
        this.Instance = VulkanRenderer.Singleton.Context.Device.CreateImage(createInfo);
        this.Instance.GetMemoryRequirements(out var memRequirements);

        var memoryType = VulkanRenderer.Singleton.Context.FindMemoryType(memRequirements.MemoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType,
        };

        var deviceMemory = VulkanRenderer.Singleton.Context.Device.AllocateMemory(memoryAllocateInfo);

        this.Instance.BindMemory(deviceMemory, 0);

        this.Extent        = createInfo.Extent;
        this.Format        = createInfo.Format;
        this.Type          = createInfo.ImageType;
        this.Usage         = createInfo.Usage;
        this.InitialLayout = createInfo.InitialLayout;
        this.Samples       = createInfo.Samples;
        this.FinalLayout   = finalLayout;

        this.allocation = new()
        {
            Alignment  = memRequirements.Alignment,
            Memory     = deviceMemory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = memRequirements.Size,
        };

        if (finalLayout != createInfo.InitialLayout)
        {
            this.TransitionLayout(createInfo.InitialLayout, finalLayout);
        }
    }

    public static Image[] Create(in VkImageCreateInfo createInfo, int count)
    {
        var images = new Image[count];

        for (var i = 0; i < count; i++)
        {
            images[i] = new(createInfo);
        }

        return images;
    }

    private unsafe Buffer ReadBuffer(VkImageAspectFlags aspectMask, out nint data)
    {
        var size = (ulong)(this.Extent.Width * this.Extent.Height * this.BytesPerPixel);

        using var buffer = new Buffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        this.CopyToBuffer(buffer, aspectMask);

        buffer.Allocation.Memory.Map(0, size, 0, out data);

        return buffer;
    }

    private unsafe NativeArray<T> ReadBuffer<T>(VkImageAspectFlags aspectMask = VkImageAspectFlags.Color) where T : unmanaged, INumber<T>
    {
        using var buffer = this.ReadBuffer(aspectMask, out var data);

        var pixels = new NativeArray<T>(this.Extent.Width * this.Extent.Height);

        try
        {
            var source = new Span<byte>(data.ToPointer(), pixels.Length * this.BytesPerPixel);

            source.CopyTo(this.BytesPerPixel, pixels.AsSpan(), true);

            return pixels;
        }
        catch
        {
            pixels.Dispose();

            throw;
        }
    }

    protected override void OnDisposed()
    {
        if (this.allocation != null)
        {
            this.allocation.Dispose();
            this.Instance.Dispose();
        }
    }

    public unsafe void ClearColor(in Color color, VkImageLayout layout)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        var clearColor = new VkClearColorValue();

        clearColor.Float32[0] = color.R;
        clearColor.Float32[1] = color.G;
        clearColor.Float32[2] = color.B;
        clearColor.Float32[3] = color.A;

        var range = new VkImageSubresourceRange
        {
            AspectMask = this.Aspect,
            LevelCount = 1,
            LayerCount = 1,
        };

        this.TransitionLayout(commandBuffer, this.InitialLayout, VkImageLayout.TransferDstOptimal);

        commandBuffer.ClearImageColor(this, VkImageLayout.TransferDstOptimal, clearColor, [range]);

        this.TransitionLayout(commandBuffer, VkImageLayout.TransferDstOptimal, layout);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyFromBuffer(Buffer buffer)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        this.CopyFromBuffer(commandBuffer, buffer);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyFromBuffer(CommandBuffer commandBuffer, Buffer buffer)
    {
        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent      = this.Extent,
            ImageSubresource = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
            }
        };

        commandBuffer.Instance.CopyBufferToImage(buffer, this.Instance, VkImageLayout.TransferDstOptimal, [bufferImageCopy]);
    }

    public void CopyToBuffer(Buffer buffer, VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        this.TransitionLayout(commandBuffer, VkImageLayout.General, VkImageLayout.TransferSrcOptimal);

        this.CopyToBuffer(commandBuffer, buffer, aspectMask);

        this.TransitionLayout(commandBuffer, VkImageLayout.TransferSrcOptimal, VkImageLayout.General);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyToBuffer(CommandBuffer commandBuffer, Buffer buffer, VkImageAspectFlags aspectMask = VkImageAspectFlags.Color)
    {
        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent      = this.Extent,
            ImageSubresource = new()
            {
                AspectMask = aspectMask,
                LayerCount = 1,
            }
        };

        commandBuffer.Instance.CopyImageToBuffer(this.Instance, VkImageLayout.TransferSrcOptimal, buffer.Instance, [bufferImageCopy]);
    }

    public unsafe NativeArray<uint> ReadBuffer(VkImageAspectFlags aspectMask = VkImageAspectFlags.Color) =>
        this.ReadBuffer<uint>(aspectMask);

    public unsafe NativeArray<ulong> ReadBuffer64bits(VkImageAspectFlags aspectMask = VkImageAspectFlags.Color) =>
        this.ReadBuffer<ulong>(aspectMask);

    public void TransitionLayout(VkImageLayout oldLayout, VkImageLayout newLayout)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        this.TransitionLayout(commandBuffer, oldLayout, newLayout);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    public void TransitionLayout(CommandBuffer commandBuffer, VkImageLayout oldLayout, VkImageLayout newLayout)
    {
        VkPipelineStageFlags sourceStage;
        VkPipelineStageFlags destinationStage;
        VkAccessFlags        srcAccessMask;
        VkAccessFlags        dstAccessMask;

        switch ((oldLayout, newLayout))
        {
            case (VkImageLayout.General,               VkImageLayout.TransferDstOptimal):
            case (VkImageLayout.General,               VkImageLayout.TransferSrcOptimal):
            case (VkImageLayout.ShaderReadOnlyOptimal, VkImageLayout.General):
            case (VkImageLayout.TransferDstOptimal,    VkImageLayout.General):
            case (VkImageLayout.TransferSrcOptimal,    VkImageLayout.General):
            case (VkImageLayout.Undefined,             VkImageLayout.General):
            case (VkImageLayout.Undefined,             VkImageLayout.TransferDstOptimal):
                srcAccessMask = default;
                dstAccessMask = VkAccessFlags.TransferWrite;

                sourceStage      = VkPipelineStageFlags.TopOfPipe;
                destinationStage = VkPipelineStageFlags.Transfer;

                break;
            case (VkImageLayout.Undefined, VkImageLayout.ColorAttachmentOptimal):
                srcAccessMask = default;
                dstAccessMask = VkAccessFlags.ColorAttachmentWrite;

                sourceStage      = VkPipelineStageFlags.TopOfPipe;
                destinationStage = VkPipelineStageFlags.ColorAttachmentOutput;

                break;
            case (VkImageLayout.TransferDstOptimal, VkImageLayout.ShaderReadOnlyOptimal):
            case (VkImageLayout.Undefined,          VkImageLayout.ShaderReadOnlyOptimal):
                srcAccessMask = VkAccessFlags.TransferWrite;
                dstAccessMask = VkAccessFlags.ShaderRead;

                sourceStage      = VkPipelineStageFlags.Transfer;
                destinationStage = VkPipelineStageFlags.FragmentShader;

                break;
            case (VkImageLayout.ColorAttachmentOptimal, VkImageLayout.PresentSrcKHR):
                srcAccessMask = VkAccessFlags.ColorAttachmentWrite;
                dstAccessMask = VkAccessFlags.MemoryRead;

                sourceStage      = VkPipelineStageFlags.ColorAttachmentOutput;
                destinationStage = VkPipelineStageFlags.BottomOfPipe;

                break;
            default:
                throw new Exception($"Unsupported layout transition! - {oldLayout} -> {newLayout}");
        }

        var imageMemoryBarrier = new VkImageMemoryBarrier
        {
            DstAccessMask       = dstAccessMask,
            DstQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            Image               = this.Instance.Handle,
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

        commandBuffer.Instance.PipelineBarrier(sourceStage, destinationStage, default, [], [], [imageMemoryBarrier]);

        this.FinalLayout = newLayout;
    }

    public void Update(scoped ReadOnlySpan<byte> data)
    {
        using var buffer = new Buffer((ulong)data.Length, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        buffer.Allocation.Memory.Write(0, 0, data);

        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        this.TransitionLayout(commandBuffer, VkImageLayout.Undefined, VkImageLayout.TransferDstOptimal);

        this.CopyFromBuffer(commandBuffer, buffer);

        this.TransitionLayout(commandBuffer, VkImageLayout.TransferDstOptimal, VkImageLayout.ShaderReadOnlyOptimal);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }
}
