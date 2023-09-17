using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created graphics pipeline</para>
/// <para>The parameters basePipelineHandle and basePipelineIndex are described in more detail in Pipeline Derivatives.</para>
/// <para>If any shader stage fails to compile, the compile log will be reported back to the application, and <see cref="VkResult.VK_ERROR_INVALID_SHADER_NV"/> will be generated.</para>
/// <remarks>Note: With <see cref="VkExtExtendedDynamicState3"/>, it is possible that many of the <see cref="VkGraphicsPipelineCreateInfo"/> members above can be NULL because all their state is dynamic and therefore ignored. This is optional so the application can still use a valid pointer if it needs to set the pNext or flags fields to specify state for other extensions.</remarks>
/// <para>The state required for a graphics pipeline is divided into <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-vertex-input">vertex input state</see>, <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization shader state</see>, <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-fragment-shader">fragment shader state</see>, and <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-fragment-output">fragment output state</see>.</para>
/// <para>Vertex Input State</para>
/// <para>Vertex input state is defined by:</para>
/// <list type="bullet">
/// <item><see cref="VkPipelineVertexInputStateCreateInfo"/></item>
/// <item><see cref="VkPipelineInputAssemblyStateCreateInfo"/></item>
/// </list>
/// <para>If this pipeline specifies <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> either directly or by including it as a pipeline library and its pStages includes a vertex shader, this state must be specified to create a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see>.</para>
/// <para>If a pipeline includes <see cref="VkGraphicsPipelineLibraryFlagBitsEXT.VK_GRAPHICS_PIPELINE_LIBRARY_VERTEX_INPUT_INTERFACE_BIT_EXT"/> in <see cref="VkGraphicsPipelineLibraryCreateInfoEXT.flags"/> either explicitly or as a default, and either the conditions requiring this state for a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see> are met or this pipeline does not specify <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> in any way, that pipeline must specify this state directly.</para>
/// <para>Pre-Rasterization Shader State</para>
/// <para>Pre-rasterization shader state is defined by:</para>
/// <list type="bullet">
/// <item>
/// <para><see cref="VkPipelineShaderStageCreateInfo"/> entries for:</para>
/// <list type="number">
/// <item>Vertex shaders</item>
/// <item>Tessellation control shaders</item>
/// <item>Tessellation evaluation shaders</item>
/// <item>Geometry shaders</item>
/// <item>Task shaders</item>
/// <item>Mesh shaders</item>
/// </list>
/// </item>
/// <item>Within the <see cref="VkPipelineLayout"/>, all descriptor sets with pre-rasterization shader bindings if <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/> was specified.</item>
/// <item>If <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/> was not specified, the full pipeline layout must be specified.</item>
/// <item><see cref="VkPipelineViewportStateCreateInfo"/></item>
/// <item><see cref="VkPipelineRasterizationStateCreateInfo"/></item>
/// <item><see cref="VkPipelineTessellationStateCreateInfo"/></item>
/// <item><see cref="VkRenderPass"/> and subpass parameter</item>
/// <item>The viewMask parameter of <see cref="VkPipelineRenderingCreateInfo"/> (formats are ignored)</item>
/// <item><see cref="VkPipelineDiscardRectangleStateCreateInfoEXT"/></item>
/// <item><see cref="VkPipelineFragmentShadingRateStateCreateInfoKHR"/></item>
/// </list>
/// <para>This state must be sp</para>ecified to create a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see>.
/// <para>If either the pNext c</para>hain includes a <see cref="VkGraphicsPipelineLibraryCreateInfoEXT"/> structure with <see cref="VkGraphicsPipelineLibraryFlagBitsEXT.VK_GRAPHICS_PIPELINE_LIBRARY_PRE_RASTERIZATION_SHADERS_BIT_EXT"/> included in flags, or it is not specified and would default to include that value, this state must be specified in the pipeline.
/// <para>Fragment Shader State</para>
/// <para>Fragment shader state is defined by:</para>
/// <list type="bullet">
/// <item>A <see cref="VkPipelineShaderStageCreateInfo"/> entry for the fragment shader</item>
/// <item>
/// <para>Within the <see cref="VkPipelineLayout"/>, all descriptor sets with fragment shader bindings if <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/> was specified.</para>
/// <list type="number">
/// <item>If <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/> was not specified, the full pipeline layout must be specified.</item>
/// </list>
/// </item>
/// <item><see cref="VkPipelineMultisampleStateCreateInfo"/> if sample shading is enabled or renderpass is not default</item>
/// <item><see cref="VkPipelineDepthStencilStateCreateInfo"/></item>
/// <item><see cref="VkRenderPass"/> and subpass parameter</item>
/// <item>The viewMask parameter of <see cref="VkPipelineRenderingCreateInfo"/> (formats are ignored)</item>
/// <item><see cref="VkPipelineFragmentShadingRateStateCreateInfoKHR"/></item>
/// <item><see cref="VkPipelineFragmentShadingRateEnumStateCreateInfoNV"/></item>
/// <item><see cref="VkPipelineRepresentativeFragmentTestStateCreateInfoNV"/></item>
/// <item>Inclusion/omission of the <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/> flag</item>
/// <item>Inclusion/omission of the <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT"/> flag</item>
/// </list>
/// <para>If a pipeline specifies <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> either directly or by including it as a pipeline library and rasterizerDiscardEnable is set to false or <see cref="VkDynamicState.VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE"/> is used, this state must be specified to create a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see>.</para>
/// <para>If a pipeline includes <see cref="VkGraphicsPipelineLibraryFlagBitsEXT.VK_GRAPHICS_PIPELINE_LIBRARY_FRAGMENT_SHADER_BIT_EXT"/> in <see cref="VkGraphicsPipelineLibraryCreateInfoEXT.flags"/> either explicitly or as a default, and either the conditions requiring this state for a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see> are met or this pipeline does not specify <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> in any way, that pipeline must specify this state directly.</para>
/// <para>Fragment Output State</para>
/// <para>Fragment output state is defined by:</para>
/// <list type="bullet">
/// <item><see cref="VkPipelineColorBlendStateCreateInfo"/></item>
/// <item><see cref="VkRenderPass"/> and subpass parameter</item>
/// <item><see cref="VkPipelineMultisampleStateCreateInfo"/></item>
/// <item><see cref="VkPipelineRenderingCreateInfo"/></item>
/// <item><see cref="VkAttachmentSampleCountInfoAMD"/></item>
/// <item><see cref="VkAttachmentSampleCountInfoNV"/></item>
/// <item>Inclusion/omission of the <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_COLOR_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT"/> and <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_DEPTH_STENCIL_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT"/> flags</item>
/// </list>
/// <para>If a pipeline specifies <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> either directly or by including it as a pipeline library and rasterizerDiscardEnable is set to false or <see cref="VkDynamicState.VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE"/> is used, this state must be specified to create a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see>.</para>
/// <para>If a pipeline includes <see cref="VkGraphicsPipelineLibraryFlagBitsEXT.VK_GRAPHICS_PIPELINE_LIBRARY_FRAGMENT_OUTPUT_INTERFACE_BIT_EXT"/> in <see cref="VkGraphicsPipelineLibraryCreateInfoEXT"/>::flags either explicitly or as a default, and either the conditions requiring this state for a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see> are met or this pipeline does not specify <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization state</see> in any way, that pipeline must specify this state directly.</para>
/// <para>Dynamic State</para>
/// <para>Dynamic state values set via pDynamicState must be ignored if the state they correspond to is not otherwise statically set by one of the state subsets used to create the pipeline. Additionally, setting dynamic state values must not modify whether state in a linked library is static or dynamic; this is set and unchangeable when the library is created. For example, if a pipeline only included <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization shader state</see>, then any dynamic state value corresponding to depth or stencil testing has no effect. Any linked library that has dynamic state enabled that same dynamic state must also be enabled in all the other linked libraries to which that dynamic state applies.</para>
/// <para>Complete Graphics Pipelines</para>
/// <para>A <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-complete">complete graphics pipeline</see> always includes <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics-subsets-pre-rasterization">pre-rasterization shader</see> state, with other subsets included depending on that state as specified in the above sections.</para>
/// <para>Graphics Pipeline Library Layouts</para>
/// <para>If different subsets are linked together with pipeline layouts created with <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/>, the final effective pipeline layout is effectively the union of the linked pipeline layouts. When binding descriptor sets for this pipeline, the pipeline layout used must be compatible with this union. This pipeline layout can be overridden when linking with <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/> by providing a <see cref="VkPipelineLayout"/> that is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-compatibility">compatible</see> with this union other than <see cref="VkPipelineLayoutCreate.VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT"/>, or when linking without <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/> by providing a <see cref="VkPipelineLayout"/> that is fully <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-compatibility">compatible</see> with this union.</para>
/// <para>If a <see cref="VkPipelineCreateFlags2CreateInfoKHR"/> structure is present in the pNext chain, <see cref="VkPipelineCreateFlags2CreateInfoKHR.flags"/> from that structure is used instead of flags from this structure.</para>
/// </summary>
public unsafe struct VkGraphicsPipelineCreateInfo
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
    /// A bitmask of <see cref="VkPipelineCreateFlagBits"/> specifying how the pipeline will be generated.
    /// </summary>
    public VkPipelineCreateFlags flags;

    /// <summary>
    /// The number of entries in the pStages array.
    /// </summary>
    public uint stageCount;

    /// <summary>
    /// A pointer to an array of stageCount <see cref="VkPipelineShaderStageCreateInfo"/> structures describing the set of the shader stages to be included in the graphics pipeline.
    /// </summary>
    public VkPipelineShaderStageCreateInfo* pStages;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineVertexInputStateCreateInfo"/> structure. It is ignored if the pipeline includes a mesh shader stage. It can be NULL if the pipeline is created with the <see cref="VkDynamicState.VK_DYNAMIC_STATE_VERTEX_INPUT_EXT"/> dynamic state set.
    /// </summary>
    public VkPipelineVertexInputStateCreateInfo* pVertexInputState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineInputAssemblyStateCreateInfo"/> structure which determines input assembly behavior for vertex shading, as described in Drawing Commands. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with both <see cref="VkDynamicState.VK_DYNAMIC_STATE_PRIMITIVE_RESTART_ENABLE"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_PRIMITIVE_TOPOLOGY"/> dynamic states set and dynamicPrimitiveTopologyUnrestricted is true. It is ignored if the pipeline includes a mesh shader stage.
    /// </summary>
    public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineTessellationStateCreateInfo"/> structure defining tessellation state used by tessellation shaders. It can be NULL if the pipeline is created with the <see cref="VkDynamicState.VK_DYNAMIC_STATE_PATCH_CONTROL_POINTS_EXT"/> dynamic state set.
    /// </summary>
    public VkPipelineTessellationStateCreateInfo* pTessellationState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineViewportStateCreateInfo"/> structure defining viewport state used when rasterization is enabled. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with both <see cref="VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT_WITH_COUNT"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_SCISSOR_WITH_COUNT"/> dynamic states set.
    /// </summary>
    public VkPipelineViewportStateCreateInfo* pViewportState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineRasterizationStateCreateInfo"/> structure defining rasterization state. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with all of <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_CLAMP_ENABLE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_POLYGON_MODE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_CULL_MODE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_FRONT_FACE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_BIAS_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_BIAS"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_LINE_WIDTH"/> dynamic states set.
    /// </summary>
    public VkPipelineRasterizationStateCreateInfo* pRasterizationState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineMultisampleStateCreateInfo"/> structure defining multisample state used when rasterization is enabled. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with all of <see cref="VkDynamicState.VK_DYNAMIC_STATE_RASTERIZATION_SAMPLES_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_SAMPLE_MASK_EXT"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_ALPHA_TO_COVERAGE_ENABLE_EXT"/> dynamic states set, and either alphaToOne is disabled on the device or <see cref="VkDynamicState.VK_DYNAMIC_STATE_ALPHA_TO_ONE_ENABLE_EXT"/> is set, in which case <see cref="VkPipelineMultisampleStateCreateInfo.sampleShadingEnable"/> is assumed to be false.
    /// </summary>
    public VkPipelineMultisampleStateCreateInfo* pMultisampleState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineDepthStencilStateCreateInfo"/> structure defining depth/stencil state used when rasterization is enabled for depth or stencil attachments accessed during rendering. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with all of <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_TEST_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_WRITE_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_COMPARE_OP"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_BOUNDS_TEST_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_STENCIL_TEST_ENABLE"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_STENCIL_OP"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_DEPTH_BOUNDS"/> dynamic states set.
    /// </summary>
    public VkPipelineDepthStencilStateCreateInfo* pDepthStencilState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineColorBlendStateCreateInfo"/> structure defining color blend state used when rasterization is enabled for any color attachments accessed during rendering. If the <see cref="VkExtExtendedDynamicState3"/> extension is enabled, it can be NULL if the pipeline is created with all of <see cref="VkDynamicState.VK_DYNAMIC_STATE_LOGIC_OP_ENABLE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_LOGIC_OP_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_ENABLE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_EQUATION_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_WRITE_MASK_EXT"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_BLEND_CONSTANTS"/> dynamic states set.
    /// </summary>
    public VkPipelineColorBlendStateCreateInfo* pColorBlendState;

    /// <summary>
    /// A pointer to a <see cref="VkPipelineDynamicStateCreateInfo"/> structure defining which properties of the pipeline state object are dynamic and can be changed independently of the pipeline state. This can be NULL, which means no state in the pipeline is considered dynamic.
    /// </summary>
    public VkPipelineDynamicStateCreateInfo* pDynamicState;

    /// <summary>
    /// The description of binding locations used by both the pipeline and descriptor sets used with the pipeline.
    /// </summary>
    public VkPipelineLayout layout;

    /// <summary>
    /// A handle to a render pass object describing the environment in which the pipeline will be used. The pipeline must only be used with a render pass instance compatible with the one provided. See Render Pass Compatibility for more information.
    /// </summary>
    public VkRenderPass renderPass;

    /// <summary>
    /// The index of the subpass in the render pass where this pipeline will be used.
    /// </summary>
    public uint subpass;

    /// <summary>
    /// A pipeline to derive from.
    /// </summary>
    public VkPipeline basePipelineHandle;

    /// <summary>
    /// An index into the pCreateInfos parameter to use as a pipeline to derive from.
    /// </summary>
    public int basePipelineIndex;

    public VkGraphicsPipelineCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO;
}
