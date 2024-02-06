using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolSize.html">VkDescriptorPoolSize</see>
/// </summary>
public struct VkDescriptorPoolSize
{
    public VkDescriptorType Type;
    public uint             DescriptorCount;
}
