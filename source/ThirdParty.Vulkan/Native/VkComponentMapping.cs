using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkComponentMapping.html">VkComponentMapping</see>
/// </summary>
public struct VkComponentMapping
{
    public VkComponentSwizzle R;
    public VkComponentSwizzle G;
    public VkComponentSwizzle B;
    public VkComponentSwizzle A;
}
