using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying parameters of a newly created render pass.
/// </summary>
/// <remarks>Note: Care should be taken to avoid a data race here; if any subpasses access attachments with overlapping memory locations, and one of those accesses is a write, a subpass dependency needs to be included between them.</remarks>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkRenderPassCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;
    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkRenderPassCreateFlagBits"/>
    /// </summary>
    public VkRenderPassCreateFlags flags;

    /// <summary>
    /// The number of attachments used by this render pass.
    /// </summary>
    public uint attachmentCount;

    /// <summary>
    /// A pointer to an array of attachmentCount <see cref="VkAttachmentDescription"/> structures describing the attachments used by the render pass.
    /// </summary>
    public VkAttachmentDescription* pAttachments;

    /// <summary>
    /// The number of subpasses to create.
    /// </summary>
    public uint subpassCount;

    /// <summary>
    /// A pointer to an array of subpassCount <see cref="VkSubpassDescription"/> structures describing each subpass.
    /// </summary>
    public VkSubpassDescription* pSubpasses;

    /// <summary>
    /// The number of memory dependencies between pairs of subpasses.
    /// </summary>
    public uint dependencyCount;

    /// <summary>
    /// A pointer to an array of dependencyCount <see cref="VkSubpassDependency"/> structures describing dependencies between pairs of subpasses.
    /// </summary>
    public VkSubpassDependency* pDependencies;

    public VkRenderPassCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO;
}
