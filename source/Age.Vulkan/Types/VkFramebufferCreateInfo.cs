using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created framebuffer</para>
/// <para>It is legal for a subpass to use no color or depth/stencil attachments, either because it has no attachment references or because all of them are <see cref="VK_ATTACHMENT_UNUSED"/>. This kind of subpass can use shader side effects such as image stores and atomics to produce an output. In this case, the subpass continues to use the width, height, and layers of the framebuffer to define the dimensions of the rendering area, and the rasterizationSamples from each pipeline’s <see cref="VkPipelineMultisampleStateCreateInfo"/> to define the number of samples used in rasterization; however, if <see cref="VkPhysicalDeviceFeatures.variableMultisampleRate"/> is VK_FALSE, then all pipelines to be bound with the subpass must have the same value for <see cref="VkPipelineMultisampleStateCreateInfo.rasterizationSamples"/>. In all such cases, rasterizationSamples must be a bit value that is set in <see cref="VkPhysicalDeviceLimits.framebufferNoAttachmentsSampleCounts"/>.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkFramebufferCreateInfo
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
    /// A bitmask of <see cref="VkFramebufferCreateFlagBits"/>
    /// </summary>
    public VkFramebufferCreateFlags flags;

    /// <summary>
    /// A render pass defining what render passes the framebuffer will be compatible with. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-compatibility">Render Pass Compatibility</see> for details.
    /// </summary>
    public VkRenderPass renderPass;

    /// <summary>
    /// The number of attachments.
    /// </summary>
    public uint attachmentCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkImageView"/> handles, each of which will be used as the corresponding attachment in a render pass instance. If flags includes <see cref="VkFramebufferCreateFlagBits.VK_FRAMEBUFFER_CREATE_IMAGELESS_BIT"/>, this parameter is ignored.
    /// </summary>
    public VkImageView* pAttachments;

    /// <summary>
    /// <see cref="width"/>, <see cref="height"/> and <see cref="layers"/> define the dimensions of the framebuffer. If the render pass uses multiview, then layers must be one and each attachment requires a number of layers that is greater than the maximum bit index set in the view mask in the subpasses in which it is used.
    /// </summary>
    public uint width;

    /// <inheritdoc cref="width" />
    public uint height;

    /// <inheritdoc cref="width" />
    public uint layers;

    public VkFramebufferCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
}
