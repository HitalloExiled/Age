using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageMemoryBarrier.html">VkImageMemoryBarrier</see>
/// </summary>
public unsafe struct VkImageMemoryBarrier
{
    public readonly VkStructureType SType;

    public void*                   PNext;
    public VkAccessFlags           SrcAccessMask;
    public VkAccessFlags           DstAccessMask;
    public VkImageLayout           OldLayout;
    public VkImageLayout           NewLayout;
    public uint                    SrcQueueFamilyIndex;
    public uint                    DstQueueFamilyIndex;
    public VkHandle<VkImage>       Image;
    public VkImageSubresourceRange SubresourceRange;

    public VkImageMemoryBarrier() =>
        this.SType = VkStructureType.ImageMemoryBarrier;
}
