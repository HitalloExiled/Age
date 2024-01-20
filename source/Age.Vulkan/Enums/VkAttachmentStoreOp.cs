namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Specify how contents of an attachment are stored to memory at the end of a subpass.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkAttachmentStoreOp
{
    /// <summary>
    /// Specifies the contents generated during the render pass and within the render area are written to memory. For attachments with a depth/stencil format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT"/>. For attachments with a color format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT"/>.
    /// </summary>
    VK_ATTACHMENT_STORE_OP_STORE = 0,

    /// <summary>
    /// Specifies the contents within the render area are not needed after rendering, and may be discarded; the contents of the attachment will be undefined inside the render area. For attachments with a depth/stencil format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT"/>. For attachments with a color format, this uses the access type <see cref="VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT"/>.
    /// </summary>
    VK_ATTACHMENT_STORE_OP_DONT_CARE = 1,

    /// <summary>
    /// <para>Specifies the contents within the render area are not accessed by the store operation as long as no values are written to the attachment during the render pass. If values are written during the render pass, this behaves identically to <see cref="VK_ATTACHMENT_STORE_OP_DONT_CARE"/> and with matching access semantics.</para>
    /// <remarks><see cref="VK_ATTACHMENT_STORE_OP_DONT_CARE"/> can cause contents generated during previous render passes to be discarded before reaching memory, even if no write to the attachment occurs during the current render pass.</remarks>
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_ATTACHMENT_STORE_OP_NONE = 1000301000,

    /// <inheritdoc cref="VK_ATTACHMENT_STORE_OP_NONE" />
    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_ATTACHMENT_STORE_OP_NONE_KHR = VK_ATTACHMENT_STORE_OP_NONE,

    /// <inheritdoc cref="VK_ATTACHMENT_STORE_OP_NONE" />
    /// <remarks>Provided by VK_QCOM_render_pass_store_ops</remarks>
    VK_ATTACHMENT_STORE_OP_NONE_QCOM = VK_ATTACHMENT_STORE_OP_NONE,

    /// <inheritdoc cref="VK_ATTACHMENT_STORE_OP_NONE" />
    /// <remarks>Provided by VK_EXT_load_store_op_none</remarks>
    VK_ATTACHMENT_STORE_OP_NONE_EXT = VK_ATTACHMENT_STORE_OP_NONE,
}
