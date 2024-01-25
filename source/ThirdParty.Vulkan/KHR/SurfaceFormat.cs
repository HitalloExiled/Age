

using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;

namespace ThirdParty.Vulkan.KHR;

#pragma warning disable IDE0001

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSurfaceFormatKHR.html">VkSurfaceFormatKHR</see>
/// </summary>
public class SurfaceFormat
{
    public Format     Format { get; set; }
    public ColorSpace ColorSpace { get; set; }
}
