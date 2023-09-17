using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying render pass begin information.</para>
/// <para>renderArea is the render area that is affected by the render pass instance. The effects of attachment load, store and multisample resolve operations are restricted to the pixels whose x and y coordinates fall within the render area on all attachments. The render area extends to all layers of framebuffer. The application must ensure (using scissor if necessary) that all rendering is contained within the render area. The render area, after any transform specified by <see cref="VkRenderPassTransformBeginInfoQCOM.transform"/> is applied, must be contained within the framebuffer dimensions.</para>
/// <para>If <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#vertexpostproc-renderpass-transform">render pass transform</see> is enabled, then renderArea must equal the framebuffer pre-transformed dimensions. After renderArea has been transformed by <see cref="VkRenderPassTransformBeginInfoQCOM.transform"/>, the resulting render area must be equal to the framebuffer dimensions.</para>
/// <para>If multiview is enabled in renderPass, and <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-multiview-per-view-render-areas">multiviewPerViewRenderAreas</see> feature is enabled, and there is an instance of <see cref="VkMultiviewPerViewRenderAreasRenderPassBeginInfoQCOM"/> included in the pNext chain with perViewRenderAreaCount not equal to 0, then the elements of <see cref="VkMultiviewPerViewRenderAreasRenderPassBeginInfoQCOM.pPerViewRenderAreas"/> override renderArea and define a render area for each view. In this case, renderArea must be set to an area at least as large as the union of all the per-view render areas.</para>
/// <para>If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-subpassShading">subpassShading</see> feature is enabled, then renderArea must equal the framebuffer dimensions.</para>
/// <remarks>Note: There may be a performance cost for using a render area smaller than the framebuffer, unless it matches the render area granularity for the render pass.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkRenderPassBeginInfo
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// The render pass to begin an instance of.
    /// </summary>
    public VkRenderPass renderPass;

    /// <summary>
    /// The framebuffer containing the attachments that are used with the render pass.
    /// </summary>
    public VkFramebuffer framebuffer;

    /// <summary>
    /// The render area that is affected by the render pass instance, and is described in more detail below.
    /// </summary>
    public VkRect2D renderArea;

    /// <summary>
    /// The number of elements in pClearValues.
    /// </summary>
    public uint clearValueCount;

    /// <summary>
    /// A pointer to an array of clearValueCount <see cref="VkClearValue"/> structures containing clear values for each attachment, if the attachment uses a loadOp value of <see cref="VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR"/> or if the attachment has a depth/stencil format and uses a stencilLoadOp value of <see cref="VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR"/>. The array is indexed by attachment number. Only elements corresponding to cleared attachments are used. Other elements of pClearValues are ignored.
    /// </summary>
    public VkClearValue* pClearValues;

    public VkRenderPassBeginInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO;
}
