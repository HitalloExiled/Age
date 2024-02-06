using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageViewCreateInfo.html">VkImageViewCreateInfo</see>
/// </summary>
public unsafe struct VkImageViewCreateInfo
{
    public readonly VkStructureType SType;

    public void*                   PNext;
    public VkImageViewCreateFlags  Flags;
    public VkHandle<VkImage>       Image;
    public VkImageViewType         ViewType;
    public VkFormat                Format;
    public VkComponentMapping      Components;
    public VkImageSubresourceRange SubresourceRange;

    public VkImageViewCreateInfo() =>
        this.SType = VkStructureType.ImageViewCreateInfo;
}
