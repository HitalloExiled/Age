using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying an attachment description.</para>
/// <para>If the attachment uses a color format, then loadOp and storeOp are used, and stencilLoadOp and stencilStoreOp are ignored. If the format has depth and/or stencil components, loadOp and storeOp apply only to the depth data, while stencilLoadOp and stencilStoreOp define how the stencil data is handled. loadOp and stencilLoadOp define the load operations for the attachment. storeOp and stencilStoreOp define the store operations for the attachment. If an attachment is not used by any subpass, loadOp, storeOp, stencilStoreOp, and stencilLoadOp will be ignored for that attachment, and no load or store ops will be performed. However, any transition specified by initialLayout and finalLayout will still be executed.</para>
/// <para>If flags includes <see cref="VkAttachmentDescriptionFlagBits.VK_ATTACHMENT_DESCRIPTION_MAY_ALIAS_BIT"/>, then the attachment is treated as if it shares physical memory with another attachment in the same render pass. This information limits the ability of the implementation to reorder certain operations (like layout transitions and the loadOp) such that it is not improperly reordered against other uses of the same physical memory via a different attachment. This is described in more detail below.</para>
/// <para>If a render pass uses multiple attachments that alias the same device memory, those attachments must each include the <see cref="VkAttachmentDescriptionFlagBits.VK_ATTACHMENT_DESCRIPTION_MAY_ALIAS_BIT"/> bit in their attachment description flags. Attachments aliasing the same memory occurs in multiple ways:</para>
/// <list type="bullet">
/// <item>Multiple attachments being assigned the same image view as part of framebuffer creation.</item>
/// <item>Attachments using distinct image views that correspond to the same image subresource of an image.</item>
/// <item>Attachments using views of distinct image subresources which are bound to overlapping memory ranges.</item>
/// </list>
/// <remarks>Render passes must include subpass dependencies (either directly or via a subpass dependency chain) between any two subpasses that operate on the same attachment or aliasing attachments and those subpass dependencies must include execution and memory dependencies separating uses of the aliases, if at least one of those subpasses writes to one of the aliases. These dependencies must not include the <see cref="VkDependencyFlagBits.VK_DEPENDENCY_BY_REGION_BIT"/> if the aliases are views of distinct image subresources which overlap in memory.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkAttachmentDescription
{
    /// <summary>
    /// A bitmask of VkAttachmentDescriptionFlagBits specifying additional properties of the attachment.
    /// </summary>
    public VkAttachmentDescriptionFlags flags;

    /// <summary>
    /// A VkFormat value specifying the format of the image view that will be used for the attachment.
    /// </summary>
    public VkFormat format;

    /// <summary>
    /// A VkSampleCountFlagBits value specifying the number of samples of the image.
    /// </summary>
    public VkSampleCountFlagBits samples;

    /// <summary>
    /// A VkAttachmentLoadOp value specifying how the contents of color and depth components of the attachment are treated at the beginning of the subpass where it is first used.
    /// </summary>
    public VkAttachmentLoadOp loadOp;

    /// <summary>
    /// A <see cref="VkAttachmentStoreOp"/> value specifying how the contents of color and depth components of the attachment are treated at the end of the subpass where it is last used.
    /// </summary>
    public VkAttachmentStoreOp storeOp;

    /// <summary>
    /// A <see cref="VkAttachmentLoadOp"/> value specifying how the contents of stencil components of the attachment are treated at the beginning of the subpass where it is first used.
    /// </summary>
    public VkAttachmentLoadOp stencilLoadOp;

    /// <summary>
    /// A <see cref="VkAttachmentStoreOp"/> value specifying how the contents of stencil components of the attachment are treated at the end of the last subpass where it is used.
    /// </summary>
    public VkAttachmentStoreOp stencilStoreOp;

    /// <summary>
    /// The layout the attachment image subresource will be in when a render pass instance begins.
    /// </summary>
    public VkImageLayout initialLayout;

    // The layout the attachment image subresource will be transitioned to when a render pass instance ends.
    public VkImageLayout finalLayout;
}
