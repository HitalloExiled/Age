using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorImageInfo.html">VkDescriptorImageInfo</see>
/// </summary>
public struct VkDescriptorImageInfo
{
    public VkHandle<VkSampler>   Sampler;
    public VkHandle<VkImageView> ImageView;
    public VkImageLayout         ImageLayout;
}
