namespace Age.Vulkan.Native.Flags;

/// <summary>
/// <para>Bitmask specifying usage of a subpass.</para>
/// <remarks>Note: Shader resolve operations allow for custom resolve operations, but overdrawing pixels may have a performance and/or power cost. Furthermore, since the content of any depth stencil attachment or color attachment is undefined at the beginning of a shader resolve subpass, any depth testing, stencil testing, or blending operation which sources these undefined values also has undefined result value.</remarks>
/// </summary>
//// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkSubpassDescriptionFlagBits
{
    /// <summary>
    /// Specifies that shaders compiled for this subpass write the attributes for all views in a single invocation of each pre-rasterization shader stage. All pipelines compiled against a subpass that includes this bit must write per-view attributes to the *PerViewNV[] shader outputs, in addition to the non-per-view (e.g. Position) outputs.
    /// </summary>
    /// <remarks>Provided by VK_NVX_multiview_per_view_attributes</remarks>
    VK_SUBPASS_DESCRIPTION_PER_VIEW_ATTRIBUTES_BIT_NVX = 0x00000001,

    /// <summary>
    /// Specifies that shaders compiled for this subpass use per-view positions which only differ in value in the x component. Per-view viewport mask can also be used.
    /// </summary>
    /// <remarks>Provided by VK_NVX_multiview_per_view_attributes</remarks>
    VK_SUBPASS_DESCRIPTION_PER_VIEW_POSITION_X_ONLY_BIT_NVX = 0x00000002,

    /// <summary>
    /// Specifies that the framebuffer region is the fragment region, that is, the minimum region dependencies are by pixel rather than by sample, such that any fragment shader invocation can access any sample associated with that fragment shader invocation.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_render_pass_shader_resolve</remarks>
    VK_SUBPASS_DESCRIPTION_FRAGMENT_REGION_BIT_QCOM = 0x00000004,

    /// <summary>
    /// Specifies that the subpass performs shader resolve operations.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_render_pass_shader_resolve</remarks>
    VK_SUBPASS_DESCRIPTION_SHADER_RESOLVE_BIT_QCOM = 0x00000008,

    /// <summary>
    /// Specifies that this subpass supports pipelines created with VK_PIPELINE_COLOR_BLEND_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_ACCESS_BIT_EXT.
    /// </summary>
    /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_COLOR_ACCESS_BIT_EXT = 0x00000010,

    /// <summary>
    /// Specifies that this subpass supports pipelines created with VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT.
    /// </summary>
    /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT = 0x00000020,

    /// <summary>
    /// Specifies that this subpass supports pipelines created with VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT.
    /// </summary>
    /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT = 0x00000040,

    /// <summary>
    /// Specifies that Legacy Dithering is enabled for this subpass.
    /// </summary>
    /// <remarks>Provided by VK_EXT_legacy_dithering</remarks>
    VK_SUBPASS_DESCRIPTION_ENABLE_LEGACY_DITHERING_BIT_EXT = 0x00000080,

    /// <inheritdoc cref="VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_COLOR_ACCESS_BIT_EXT" />
    /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_COLOR_ACCESS_BIT_ARM = VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_COLOR_ACCESS_BIT_EXT,

    /// <inheritdoc cref="VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT" />
    /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_ARM = VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT,

    /// <inheritdoc cref="VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT" />
    /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
    VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_ARM = VK_SUBPASS_DESCRIPTION_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT,
}
