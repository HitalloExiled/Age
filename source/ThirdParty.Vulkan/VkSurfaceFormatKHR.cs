using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSurfaceFormatKHR.html">VkSurfaceFormatKHR</see>
/// </summary>
public struct VkSurfaceFormatKHR
{
    public VkFormat        Format;
    public VkColorSpaceKHR ColorSpace;
}
