namespace Age.Vulkan.Native.Enums.KHR;

/// <summary>
/// Alpha compositing modes supported on a device.
/// </summary>
[Flags]
public enum VkCompositeAlphaFlagBitsKHR : uint
{
    /// <summary>
    /// The alpha component, if it exists, of the images is ignored in the compositing process. Instead, the image is treated as if it has a constant alpha of 1.0.
    /// </summary>
    VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR = 0x00000001,

    /// <summary>
    /// The alpha component, if it exists, of the images is respected in the compositing process. The non-alpha components of the image are expected to already be multiplied by the alpha component by the application.
    /// </summary>
    VK_COMPOSITE_ALPHA_PRE_MULTIPLIED_BIT_KHR = 0x00000002,

    /// <summary>
    /// The alpha component, if it exists, of the images is respected in the compositing process. The non-alpha components of the image are not expected to already be multiplied by the alpha component by the application; instead, the compositor will multiply the non-alpha components of the image by the alpha component during compositing.
    /// </summary>
    VK_COMPOSITE_ALPHA_POST_MULTIPLIED_BIT_KHR = 0x00000004,

    /// <summary>
    /// The way in which the presentation engine treats the alpha component in the images is unknown to the Vulkan API. Instead, the application is responsible for setting the composite alpha blending mode using native window system commands. If the application does not set the blending mode using native window system commands, then a platform-specific default will be used.
    /// </summary>
    VK_COMPOSITE_ALPHA_INHERIT_BIT_KHR = 0x00000008,
}
