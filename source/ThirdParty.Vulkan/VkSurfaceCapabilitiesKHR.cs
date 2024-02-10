using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSurfaceCapabilitiesKHR.html">VkSurfaceCapabilitiesKHR</see>
/// </summary>
public struct VkSurfaceCapabilitiesKHR
{
    public uint                       MinImageCount;
    public uint                       MaxImageCount;
    public VkExtent2D                 CurrentExtent;
    public VkExtent2D                 MinImageExtent;
    public VkExtent2D                 MaxImageExtent;
    public uint                       MaxImageArrayLayers;
    public VkSurfaceTransformFlagsKHR SupportedTransforms;
    public VkSurfaceTransformFlagsKHR CurrentTransform;
    public VkCompositeAlphaFlagsKHR   SupportedCompositeAlpha;
    public VkImageUsageFlags          SupportedUsageFlags;
}
