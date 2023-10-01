using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying image format properties.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkFormatProperties
{
    /// <summary>
    /// A bitmask of VkFormatFeatureFlagBits specifying features supported by images created with a tiling parameter of <see cref="VkImageTiling.VK_IMAGE_TILING_LINEAR"/>.
    /// </summary>
    public VkFormatFeatureFlags linearTilingFeatures;

    /// <summary>
    /// A bitmask of VkFormatFeatureFlagBits specifying features supported by images created with a tiling parameter of <see cref="VkImageTiling.VK_IMAGE_TILING_OPTIMAL"/>.
    /// </summary>
    public VkFormatFeatureFlags optimalTilingFeatures;

    /// <summary>
    /// A bitmask of VkFormatFeatureFlagBits specifying features supported by buffers.
    /// </summary>
    public VkFormatFeatureFlags bufferFeatures;
}
