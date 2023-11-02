namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying additional properties of an attachment
/// </summary>
/// <see cref="Provided by VK_VERSION_1_0"/>
[Flags]
public enum VkAttachmentDescriptionFlagBits
{
    /// <summary>
    /// Specifies that the attachment aliases the same device memory as other attachments.
    /// </summary>
    VK_ATTACHMENT_DESCRIPTION_MAY_ALIAS_BIT = 0x00000001,
}
