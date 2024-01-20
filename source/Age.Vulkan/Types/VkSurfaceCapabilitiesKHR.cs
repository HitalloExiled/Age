using Age.Vulkan.Flags;
using Age.Vulkan.Flags.KHR;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure describing capabilities of a surface.</para>
/// <remarks>
/// <para>Supported usage flags of a presentable image when using <see cref="VkPresentModeSharedDemand.VK_PRESENT_MODE_SHARED_DEMAND_REFRESH_KHR"/> or <see cref="VkPresentModeSharedDemand.VK_PRESENT_MODE_SHARED_CONTINUOUS_REFRESH_KHR"/> presentation mode are provided by <see cref="VkSharedPresentSurfaceCapabilitiesKHR.sharedPresentSupportedUsageFlags"/>.</para>
/// <para>Formulas such as min(N, maxImageCount) are not correct, since maxImageCount may be zero.</para>
/// </remarks>
/// </summary>
public struct VkSurfaceCapabilitiesKHR
{
    /// <summary>
    /// minImageCount is the minimum number of images the specified device supports for a swapchain created for the surface, and will be at least one.
    /// </summary>
    public uint minImageCount;

    /// <summary>
    /// maxImageCount is the maximum number of images the specified device supports for a swapchain created for the surface, and will be either 0, or greater than or equal to minImageCount. A value of 0 means that there is no limit on the number of images, though there may be limits related to the total amount of memory used by presentable images.
    /// </summary>
    public uint maxImageCount;

    /// <summary>
    /// currentExtent is the current width and height of the surface, or the special value (0xFFFFFFFF, 0xFFFFFFFF) indicating that the surface size will be determined by the extent of a swapchain targeting the surface.
    /// </summary>
    public VkExtent2D currentExtent;

    /// <summary>
    /// minImageExtent contains the smallest valid swapchain extent for the surface on the specified device. The width and height of the extent will each be less than or equal to the corresponding width and height of currentExtent, unless currentExtent has the special value described above.
    /// </summary>
    public VkExtent2D minImageExtent;

    /// <summary>
    /// maxImageExtent contains the largest valid swapchain extent for the surface on the specified device. The width and height of the extent will each be greater than or equal to the corresponding width and height of minImageExtent. The width and height of the extent will each be greater than or equal to the corresponding width and height of currentExtent, unless currentExtent has the special value described above.
    /// </summary>
    public VkExtent2D maxImageExtent;

    /// <summary>
    /// maxImageArrayLayers is the maximum number of layers presentable images can have for a swapchain created for this device and surface, and will be at least one.
    /// </summary>
    public uint maxImageArrayLayers;

    /// <summary>
    /// supportedTransforms is a bitmask of <see cref="VkSurfaceTransformFlagBitsKHR"/> indicating the presentation transforms supported for the surface on the specified device. At least one bit will be set.
    /// </summary>
    public VkSurfaceTransformFlagsKHR supportedTransforms;

    /// <summary>
    /// currentTransform is <see cref="VkSurfaceTransformFlagBitsKHR"/> value indicating the surface’s current transform relative to the presentation engine’s natural orientation.
    /// </summary>
    public VkSurfaceTransformFlagBitsKHR currentTransform;

    /// <summary>
    /// supportedCompositeAlpha is a bitmask of VkCompositeAlphaFlagBitsKHR, representing the alpha compositing modes supported by the presentation engine for the surface on the specified device, and at least one bit will be set. Opaque composition can be achieved in any alpha compositing mode by either using an image format that has no alpha component, or by ensuring that all pixels in the presentable images have an alpha value of 1.0.
    /// </summary>
    public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;

    /// <summary>
    /// supportedUsageFlags is a bitmask of VkImageUsageFlagBits representing the ways the application can use the presentable images of a swapchain created with VkPresentModeKHR set to VK_PRESENT_MODE_IMMEDIATE_KHR, VK_PRESENT_MODE_MAILBOX_KHR, VK_PRESENT_MODE_FIFO_KHR or VK_PRESENT_MODE_FIFO_RELAXED_KHR for the surface on the specified device. VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT must be included in the set. Implementations may support additional usages.
    /// </summary>
    public VkImageUsageFlags supportedUsageFlags;
}
