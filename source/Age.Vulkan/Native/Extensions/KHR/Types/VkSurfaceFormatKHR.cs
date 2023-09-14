using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Extensions.KHR.Types;

/// <summary>
/// Structure describing a supported swapchain format-color space pair.
/// </summary>
public struct VkSurfaceFormatKHR
{
    /// <summary>
    /// A <see cref="VkFormat"/> that is compatible with the specified surface.
    /// </summary>
    public VkFormat format;

    /// <summary>
    /// A presentation <see cref="VkColorSpaceKHR"/> that is compatible with the surface.
    /// </summary>
    public VkColorSpaceKHR colorSpace;
}
