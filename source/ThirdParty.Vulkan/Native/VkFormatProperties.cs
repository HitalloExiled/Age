namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFormatProperties.html">VkFormatProperties</see>
/// </summary>
public struct VkFormatProperties
{
    public VkFormatFeatureFlags linearTilingFeatures;
    public VkFormatFeatureFlags optimalTilingFeatures;
    public VkFormatFeatureFlags bufferFeatures;
}
