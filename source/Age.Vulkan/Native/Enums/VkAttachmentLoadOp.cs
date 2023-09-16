namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Specify how contents of an attachment are initialized at the beginning of a subpass
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkAttachmentLoadOp
{
    /// <summary>
    /// Specifies that the previous contents of the image within the render area will be preserved as the initial values. For attachments with a depth/stencil format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT"/>. For attachments with a color format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_READ_BIT"/>.
    /// </summary>
    VK_ATTACHMENT_LOAD_OP_LOAD = 0,

    /// <summary>
    /// Specifies that the contents within the render area will be cleared to a uniform value, which is specified when a render pass instance is begun. For attachments with a depth/stencil format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT"/>. For attachments with a color format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT"/>.
    /// </summary>
    VK_ATTACHMENT_LOAD_OP_CLEAR = 1,

    /// <summary>
    /// Specifies that the previous contents within the area need not be preserved; the contents of the attachment will be undefined inside the render area. For attachments with a depth/stencil format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT"/>. For attachments with a color format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT"/>.
    /// </summary>
    VK_ATTACHMENT_LOAD_OP_DONT_CARE = 2,

    /// <summary>
    /// Specifies that the previous contents of the image will be undefined inside the render pass. No access type is used as the image is not accessed.
    /// </summary>
    /// <remarks>Provided by VK_EXT_load_store_op_none</remarks>
    VK_ATTACHMENT_LOAD_OP_NONE_EXT = 1000400000,
}
