namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorImageInfo.html">VkDescriptorImageInfo</see>
/// </summary>
public struct VkDescriptorImageInfo
{
    public VkSampler     sampler;
    public VkImageView   imageView;
    public VkImageLayout imageLayout;
}
