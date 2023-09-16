using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying a subpass description.</para>
/// <para>Each element of the pInputAttachments array corresponds to an input attachment index in a fragment shader, i.e. if a shader declares an image variable decorated with a InputAttachmentIndex value of X, then it uses the attachment provided in pInputAttachments[X]. Input attachments must also be bound to the pipeline in a descriptor set. If the attachment member of any element of pInputAttachments is <see cref="VkAttachment.VK_ATTACHMENT_UNUSED"/>, the application must not read from the corresponding input attachment index. Fragment shaders can use subpass input variables to access the contents of an input attachment at the fragment’s (x, y, layer) framebuffer coordinates. Input attachments must not be used by any subpasses within a render pass that enables render pass transform.</para>
/// <para>Each element of the pColorAttachments array corresponds to an output location in the shader, i.e. if the shader declares an output variable decorated with a Location value of X, then it uses the attachment provided in pColorAttachments[X]. If the attachment member of any element of pColorAttachments is <see cref="VkAttachment.VK_ATTACHMENT_UNUSED"/>, or if Color Write Enable has been disabled for the corresponding attachment index, then writes to the corresponding location by a fragment shader are discarded.</para>
/// <para>If flags does not include <see cref="VkSubpassDescriptionFlagBits.VK_SUBPASS_DESCRIPTION_SHADER_RESOLVE_BIT_QCOM"/>, and if pResolveAttachments is not NULL, each of its elements corresponds to a color attachment (the element in pColorAttachments at the same index), and a multisample resolve operation is defined for each attachment unless the resolve attachment index is <see cref="VkAttachment.VK_ATTACHMENT_UNUSED"/>.</para>
/// <para>Similarly, if flags does not include <see cref="VkSubpassDescriptionFlagBitsVK_SUBPASS_DESCRIPTION_SHADER_RESOLVE_BIT_QCOM"/>, and <see cref="VkSubpassDescriptionDepthStencilResolve.pDepthStencilResolveAttachment"/> is not NULL and does not have the value <see cref="VkAttachment.VK_ATTACHMENT_UNUSED"/>, it corresponds to the depth/stencil attachment in pDepthStencilAttachment, and multisample resolve operation for depth and stencil are defined by <see cref="VkSubpassDescriptionDepthStencilResolve.depthResolveMode"/> and <see cref="VkSubpassDescriptionDepthStencilResolve.stencilResolveMode"/>, respectively. If <see cref="VkSubpassDescriptionDepthStencilResolve.depthResolveMode"/> is <see cref="VkResolveModeFlagBits.VK_RESOLVE_MODE_NONE"/> or the pDepthStencilResolveAttachment does not have a depth aspect, no resolve operation is performed for the depth attachment. If <see cref="VkSubpassDescriptionDepthStencilResolve.stencilResolveMode"/> is <see cref="VkResolveModeFlagBits.VK_RESOLVE_MODE_NONE"/> or the pDepthStencilResolveAttachment does not have a stencil aspect, no resolve operation is performed for the stencil attachment.</para>
/// <para>If the image subresource range referenced by the depth/stencil attachment is created with <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_SAMPLE_LOCATIONS_COMPATIBLE_DEPTH_BIT_EXT"/>, then the multisample resolve operation uses the sample locations state specified in the sampleLocationsInfo member of the element of the <see cref="VkRenderPassSampleLocationsBeginInfoEXT.pPostSubpassSampleLocations"/> for the subpass.</para>
/// <para>If pDepthStencilAttachment is NULL, or if its attachment index is <see cref="VkAttachment.VK_ATTACHMENT_UNUSED"/>, it indicates that no depth/stencil attachment will be used in the subpass.</para>
/// <para>The contents of an attachment within the render area become undefined at the start of a subpass S if all of the following conditions are true:</para>
/// <list type="bullet">
/// <item>The attachment is used as a color, depth/stencil, or resolve attachment in any subpass in the render pass.</item>
/// <item>There is a subpass S1 that uses or preserves the attachment, and a subpass dependency from S1 to S.</item>
/// <item>The attachment is not used or preserved in subpass S.</item>
/// </list>
/// <para>In addition, the contents of an attachment within the render area become undefined at the start of a subpass S if all of the following conditions are true:</para>
/// <list type="bullet">
/// <item>VK_SUBPASS_DESCRIPTION_SHADER_RESOLVE_BIT_QCOM is set.</item>
/// <item>The attachment is used as a color or depth/stencil in the subpass.</item>
/// </list>
/// <para>Once the contents of an attachment become undefined in subpass S, they remain undefined for subpasses in subpass dependency chains starting with subpass S until they are written again. However, they remain valid for subpasses in other subpass dependency chains starting with subpass S1 if those subpasses use or preserve the attachment.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkSubpassDescription
{
    /// <summary>
    /// A bitmask of VkSubpassDescriptionFlagBits specifying usage of the subpass.
    /// </summary>
    public VkSubpassDescriptionFlags flags;

    /// <summary>
    /// A VkPipelineBindPoint value specifying the pipeline type supported for this subpass.
    /// </summary>
    public VkPipelineBindPoint pipelineBindPoint;

    /// <summary>
    /// The number of input attachments.
    /// </summary>
    public uint inputAttachmentCount;

    /// <summary>
    /// A pointer to an array of VkAttachmentReference structures defining the input attachments for this subpass and their layouts.
    /// </summary>
    public VkAttachmentReference* pInputAttachments;

    /// <summary>
    /// The number of color attachments.
    /// </summary>
    public uint colorAttachmentCount;

    /// <summary>
    /// A pointer to an array of colorAttachmentCount VkAttachmentReference structures defining the color attachments for this subpass and their layouts.
    /// </summary>
    public VkAttachmentReference* pColorAttachments;

    /// <summary>
    /// Null or a pointer to an array of colorAttachmentCount VkAttachmentReference structures defining the resolve attachments for this subpass and their layouts.
    /// </summary>
    public VkAttachmentReference* pResolveAttachments;

    /// <summary>
    /// A pointer to a VkAttachmentReference structure specifying the depth/stencil attachment for this subpass and its layout.
    /// </summary>
    public VkAttachmentReference* pDepthStencilAttachment;

    /// <summary>
    /// The number of preserved attachments.
    /// </summary>
    public uint preserveAttachmentCount;

    /// <summary>
    /// A pointer to an array of preserveAttachmentCount render pass attachment indices identifying attachments that are not used by this subpass, but whose contents must be preserved throughout the subpass.
    /// </summary>
    public uint* pPreserveAttachments;
}
