namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying framebuffer properties
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkFramebufferCreateFlagBits
{
    /// <summary>
    /// Specifies that image views are not specified, and only attachment compatibility information will be provided via a <see cref="VkFramebufferAttachmentImageInfo"/> structure.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_FRAMEBUFFER_CREATE_IMAGELESS_BIT = 0x00000001,

    /// <inheritdoc cref="VK_FRAMEBUFFER_CREATE_IMAGELESS_BIT" />
    /// <remarks>Provided by VK_KHR_imageless_framebuffer</remarks>
    VK_FRAMEBUFFER_CREATE_IMAGELESS_BIT_KHR = VK_FRAMEBUFFER_CREATE_IMAGELESS_BIT,
}
