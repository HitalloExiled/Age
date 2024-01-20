using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying an attachment reference.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkAttachmentReference
{
    /// <summary>
    /// Attachment is either an integer value identifying an attachment at the corresponding index in <see cref="VkRenderPassCreateInfo.pAttachments"/>, or <see cref="Vk.VK_ATTACHMENT_UNUSED"/> to signify that this attachment is not used.
    /// </summary>
    public uint attachment;

    /// <summary>
    /// Layout is a <see cref="VkImageLayout"/> value specifying the layout the attachment uses during the subpass.
    /// </summary>
    public VkImageLayout layout;
}
