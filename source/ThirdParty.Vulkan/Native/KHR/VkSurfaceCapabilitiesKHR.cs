namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSurfaceCapabilitiesKHR.html">VkSurfaceCapabilitiesKHR</see>
/// </summary>
public struct VkSurfaceCapabilitiesKHR
{
    public uint                          minImageCount;
    public uint                          maxImageCount;
    public VkExtent2D                    currentExtent;
    public VkExtent2D                    minImageExtent;
    public VkExtent2D                    maxImageExtent;
    public uint                          maxImageArrayLayers;
    public VkSurfaceTransformFlagsKHR    supportedTransforms;
    public VkSurfaceTransformFlagBitsKHR currentTransform;
    public VkCompositeAlphaFlagBitsKHR   supportedCompositeAlpha;
    public VkImageUsageFlags             supportedUsageFlags;
}
