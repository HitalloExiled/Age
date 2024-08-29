using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFormatProperties.html">VkFormatProperties</see>
/// </summary>
public struct VkFormatProperties
{
    public VkFormatFeatureFlags LinearTilingFeatures;
    public VkFormatFeatureFlags OptimalTilingFeatures;
    public VkFormatFeatureFlags BufferFeatures;
}
