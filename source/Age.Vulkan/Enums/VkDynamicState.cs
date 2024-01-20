using Age.Vulkan.Types;

namespace Age.Vulkan.Enums;

/// <summary>
/// Indicate which dynamic state is taken from dynamic state commands.
/// </summary>
public enum VkDynamicState
{
    /// <summary>
    /// Specifies that the pViewports state in <see cref="VkPipelineViewportStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetViewport"/> before any drawing commands. The number of viewports used by a pipeline is still specified by the viewportCount member of <see cref="VkPipelineViewportStateCreateInfo"/>.
    /// </summary>
    VK_DYNAMIC_STATE_VIEWPORT = 0,

    /// <summary>
    /// Specifies that the pScissors state in <see cref="VkPipelineViewportStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetScissor"/> before any drawing commands. The number of scissor rectangles used by a pipeline is still specified by the scissorCount member of <see cref="VkPipelineViewportStateCreateInfo"/>.
    /// </summary>
    VK_DYNAMIC_STATE_SCISSOR = 1,

    /// <summary>
    /// Specifies that the lineWidth state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetLineWidth"/> before any drawing commands that generate line primitives for the rasterizer.
    /// </summary>
    VK_DYNAMIC_STATE_LINE_WIDTH = 2,

    /// <summary>
    /// Specifies that any instance of <see cref="VkDepthBiasRepresentationInfoEXT"/> included in the pNext chain of <see cref="VkPipelineRasterizationStateCreateInfo"/> as well as the depthBiasConstantFactor, depthBiasClamp and depthBiasSlopeFactor states in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with either <see cref="Vk.CmdSetDepthBias"/> or <see cref="VkExtDepthBiasControl.CmdSetDepthBias2EXT"/> before any draws are performed with depth bias enabled.
    /// </summary>
    VK_DYNAMIC_STATE_DEPTH_BIAS = 3,

    /// <summary>
    /// Specifies that the blendConstants state in <see cref="VkPipelineColorBlendStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetBlendConstants"/> before any draws are performed with a pipeline state with <see cref="VkPipelineColorBlendAttachmentState"/> member blendEnable set to true and any of the blend functions using a constant blend color.
    /// </summary>
    VK_DYNAMIC_STATE_BLEND_CONSTANTS = 4,

    /// <summary>
    /// Specifies that the minDepthBounds and maxDepthBounds states of <see cref="VkPipelineDepthStencilStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthBounds"/> before any draws are performed with a pipeline state with <see cref="VkPipelineDepthStencilStateCreateInfo"/> member depthBoundsTestEnable set to true.
    /// </summary>
    VK_DYNAMIC_STATE_DEPTH_BOUNDS = 5,

    /// <summary>
    /// Specifies that the compareMask state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> for both front and back will be ignored and must be set dynamically with <see cref="Vk.CmdSetStencilCompareMask"/> before any draws are performed with a pipeline state with <see cref="VkPipelineDepthStencilStateCreateInfo"/> member stencilTestEnable set to true
    /// </summary>
    VK_DYNAMIC_STATE_STENCIL_COMPARE_MASK = 6,

    /// <summary>
    /// Specifies that the writeMask state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> for both front and back will be ignored and must be set dynamically with <see cref="Vk.CmdSetStencilWriteMask"/> before any draws are performed with a pipeline state with <see cref="VkPipelineDepthStencilStateCreateInfo"/> member stencilTestEnable set to true
    /// </summary>
    VK_DYNAMIC_STATE_STENCIL_WRITE_MASK = 7,

    /// <summary>
    /// Specifies that the reference state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> for both front and back will be ignored and must be set dynamically with <see cref="Vk.CmdSetStencilReference"/> before any draws are performed with a pipeline state with <see cref="VkPipelineDepthStencilStateCreateInfo"/> member stencilTestEnable set to true
    /// </summary>
    VK_DYNAMIC_STATE_STENCIL_REFERENCE = 8,

    /// <summary>
    /// Specifies that the cullMode state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetCullMode"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_CULL_MODE = 1000267000,

    /// <summary>
    /// Specifies that the frontFace state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetFrontFace"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_FRONT_FACE = 1000267001,

    /// <summary>
    /// Specifies that the topology state in <see cref="VkPipelineInputAssemblyStateCreateInfo"/> only specifies the topology class, and the specific topology order and adjacency must be set dynamically with <see cref="Vk.CmdSetPrimitiveTopology"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_PRIMITIVE_TOPOLOGY = 1000267002,

    /// <summary>
    /// Specifies that the viewportCount and pViewports state in <see cref="VkPipelineViewportStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetViewportWithCount"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_WITH_COUNT = 1000267003,

    /// <summary>
    /// Specifies that the scissorCount and pScissors state in VkPipelineViewportStateCreateInfo will be ignored and must be set dynamically with <see cref="Vk.CmdSetScissorWithCount"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_SCISSOR_WITH_COUNT = 1000267004,

    /// <summary>
    /// Specifies that the stride state in VkVertexInputBindingDescription will be ignored and must be set dynamically with <see cref="Vk.CmdBindVertexBuffers2"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_VERTEX_INPUT_BINDING_STRIDE = 1000267005,

    /// <summary>
    /// Specifies that the depthTestEnable state in VkPipelineDepthStencilStateCreateInfo will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthTestEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_DEPTH_TEST_ENABLE = 1000267006,

    /// <summary>
    /// Specifies that the depthWriteEnable state in VkPipelineDepthStencilStateCreateInfo will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthWriteEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_DEPTH_WRITE_ENABLE = 1000267007,

    /// <summary>
    /// Specifies that the depthCompareOp state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthCompareOp"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_DEPTH_COMPARE_OP = 1000267008,

    /// <summary>
    /// Specifies that the depthBoundsTestEnable state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthBoundsTestEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_DEPTH_BOUNDS_TEST_ENABLE = 1000267009,

    /// <summary>
    /// Specifies that the stencilTestEnable state in <see cref="VkPipelineDepthStencilStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetStencilTestEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_STENCIL_TEST_ENABLE = 1000267010,

    /// <summary>
    /// Specifies that the failOp, passOp, depthFailOp, and compareOp states in <see cref="VkPipelineDepthStencilStateCreateInfo"/> for both front and back will be ignored and must be set dynamically with <see cref="Vk.CmdSetStencilOp"/> before any draws are performed with a pipeline state with <see cref="VkPipelineDepthStencilStateCreateInfo"/> member stencilTestEnable set to true
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_STENCIL_OP = 1000267011,

    /// <summary>
    /// Specifies that the rasterizerDiscardEnable state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetRasterizerDiscardEnable"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE = 1000377001,

    /// <summary>
    /// Specifies that the depthBiasEnable state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetDepthBiasEnable"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_DEPTH_BIAS_ENABLE = 1000377002,

    /// <summary>
    /// Specifies that the primitiveRestartEnable state in <see cref="VkPipelineInputAssemblyStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="Vk.CmdSetPrimitiveRestartEnable"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DYNAMIC_STATE_PRIMITIVE_RESTART_ENABLE = 1000377004,

    /// <summary>
    /// Specifies that the pViewportWScalings state in <see cref="VkPipelineViewportWScalingStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkNvClipSpaceWScaling.CmdSetViewportWScalingNV"/> before any draws are performed with a pipeline state with VkPipelineViewportWScalingStateCreateInfoNV member viewportScalingEnable set to true
    /// </summary>
    /// <remarks>Provided by VK_NV_clip_space_w_scaling</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_W_SCALING_NV = 1000087000,

    /// <summary>
    /// Specifies that the pDiscardRectangles state in <see cref="VkPipelineDiscardRectangleStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtDiscardRectangles.CmdSetDiscardRectangleEXT"/> before any draw or clear commands.
    /// </summary>
    /// <remarks>Provided by VK_EXT_discard_rectangles</remarks>
    VK_DYNAMIC_STATE_DISCARD_RECTANGLE_EXT = 1000099000,

    /// <summary>
    /// Specifies that the presence of the <see cref="VkPipelineDiscardRectangleStateCreateInfoEXT"/> structure in the VkGraphicsPipelineCreateInfo chain with a discardRectangleCount greater than zero does not implicitly enable discard rectangles and they must be enabled dynamically with vkCmdSetDiscardRectangleEnableEXT before any draw commands. This is available on implementations that support at least specVersion 2 of the VK_EXT_discard_rectangles extension.
    /// </summary>
    /// <remarks>Provided by VK_EXT_discard_rectangles</remarks>
    VK_DYNAMIC_STATE_DISCARD_RECTANGLE_ENABLE_EXT = 1000099001,

    /// <summary>
    /// Specifies that the discardRectangleMode state in <see cref="VkPipelineDiscardRectangleStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtDiscardRectangles.CmdSetDiscardRectangleModeEXT"/> before any draw commands. This is available on implementations that support at least specVersion 2 of the VK_EXT_discard_rectangles extension.
    /// </summary>
    /// <remarks>Provided by VK_EXT_discard_rectangles</remarks>
    VK_DYNAMIC_STATE_DISCARD_RECTANGLE_MODE_EXT = 1000099002,

    /// <summary>
    /// Specifies that the sampleLocationsInfo state in <see cref="VkPipelineSampleLocationsStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtSampleLocations.CmdSetSampleLocationsEXT"/> before any draw or clear commands. Enabling custom sample locations is still indicated by the sampleLocationsEnable member of VkPipelineSampleLocationsStateCreateInfoEXT.
    /// </summary>
    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_DYNAMIC_STATE_SAMPLE_LOCATIONS_EXT = 1000143000,

    /// <summary>
    /// Specifies that the default stack size computation for the pipeline will be ignored and must be set dynamically with <see cref="VkKhrRayTracingPipeline.CmdSetRayTracingPipelineStackSize"/> before any ray tracing calls are performed.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_DYNAMIC_STATE_RAY_TRACING_PIPELINE_STACK_SIZE_KHR = 1000347000,

    /// <summary>
    /// Specifies that the pShadingRatePalettes state in <see cref="VkPipelineViewportShadingRateImageStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkNvShadingRateImage.CmdSetViewportShadingRatePalette"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_SHADING_RATE_PALETTE_NV = 1000164004,

    /// <summary>
    /// Specifies that the coarse sample order state in <see cref="VkPipelineViewportCoarseSampleOrderStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkNvShadingRateImage.CmdSetCoarseSampleOrder"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_COARSE_SAMPLE_ORDER_NV = 1000164006,

    /// <summary>
    /// Specifies that the the exclusive scissors must be explicitly enabled with <see cref="VkNvScissorExclusive.CmdSetExclusiveScissorEnable"/> and the exclusiveScissorCount value in <see cref="VkPipelineViewportExclusiveScissorStateCreateInfoNV"/> will not implicitly enable them. This is available on implementations that support at least specVersion 2 of the VK_NV_scissor_exclusive extension.
    /// </summary>
    /// <remarks>Provided by VK_NV_scissor_exclusive</remarks>
    VK_DYNAMIC_STATE_EXCLUSIVE_SCISSOR_ENABLE_NV = 1000205000,

    /// <summary>
    /// Specifies that the pExclusiveScissors state in <see cref="VkPipelineViewportExclusiveScissorStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkNvScissorExclusive.CmdSetExclusiveScissor"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_NV_scissor_exclusive</remarks>
    VK_DYNAMIC_STATE_EXCLUSIVE_SCISSOR_NV = 1000205001,

    /// <summary>
    /// Specifies that state in <see cref="VkPipelineFragmentShadingRateStateCreateInfoKHR"/> and <see cref="VkPipelineFragmentShadingRateEnumStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkKhrFragmentShadingRate.CmdSetFragmentShadingRate"/> or <see cref="VkNvFragmentShadingRaEnums.CmdSetFragmentShadingRateEnum"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_DYNAMIC_STATE_FRAGMENT_SHADING_RATE_KHR = 1000226000,

    /// <summary>
    /// Specifies that the lineStippleFactor and lineStipplePattern state in <see cref="VkPipelineRasterizationLineStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtLineRasterization.CmdSetLineStipple"/> before any draws are performed with a pipeline state with <see cref="VkPipelineRasterizationLineStateCreateInfoEXT"/> member stippledLineEnable set to true.
    /// </summary>
    /// <remarks>Provided by VK_EXT_line_rasterization</remarks>
    VK_DYNAMIC_STATE_LINE_STIPPLE_EXT = 1000259000,

    /// <summary>
    /// Specifies that the pVertexInputState state will be ignored and must be set dynamically with <see cref="VkExtShaderObject.CmdSetVertexInput"/> before any drawing commands
    /// </summary>
    /// <remarks>Provided by VK_EXT_vertex_input_dynamic_state</remarks>
    VK_DYNAMIC_STATE_VERTEX_INPUT_EXT = 1000352000,

    /// <summary>
    /// Specifies that the patchControlPoints state in <see cref="VkPipelineTessellationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState2.CmdSetPatchControlPoints"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_DYNAMIC_STATE_PATCH_CONTROL_POINTS_EXT = 1000377000,

    /// <summary>
    /// Specifies that the logicOp state in <see cref="VkPipelineColorBlendStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState2.CmdSetLogicOp"/> before any drawing commands.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_DYNAMIC_STATE_LOGIC_OP_EXT = 1000377003,

    /// <summary>
    /// Specifies that the pColorWriteEnables state in <see cref="VkPipelineColorWriteCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtColorWriteEnable.CmdSetColorWriteEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_color_write_enable</remarks>
    VK_DYNAMIC_STATE_COLOR_WRITE_ENABLE_EXT = 1000381000,

    /// <summary>
    /// Specifies that the domainOrigin state in <see cref="VkPipelineTessellationDomainOriginStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetTessellationDomainOrigin"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_TESSELLATION_DOMAIN_ORIGIN_EXT = 1000455002,

    /// <summary>
    /// Specifies that the depthClampEnable state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetDepthClampEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_DEPTH_CLAMP_ENABLE_EXT = 1000455003,

    /// <summary>
    /// Specifies that the polygonMode state in <see cref="VkPipelineRasterizationStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetPolygonMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_POLYGON_MODE_EXT = 1000455004,

    /// <summary>
    /// Specifies that the rasterizationSamples state in <see cref="VkPipelineMultisampleStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetRasterizationSamples"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_RASTERIZATION_SAMPLES_EXT = 1000455005,

    /// <summary>
    /// Specifies that the pSampleMask state in <see cref="VkPipelineMultisampleStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetSampleMask"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_SAMPLE_MASK_EXT = 1000455006,

    /// <summary>
    /// Specifies that the alphaToCoverageEnable state in <see cref="VkPipelineMultisampleStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetAlphaToCoverageEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_ALPHA_TO_COVERAGE_ENABLE_EXT = 1000455007,

    /// <summary>
    /// Specifies that the alphaToOneEnable state in <see cref="VkPipelineMultisampleStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetAlphaToOneEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_ALPHA_TO_ONE_ENABLE_EXT = 1000455008,

    /// <summary>
    /// Specifies that the logicOpEnable state in <see cref="VkPipelineColorBlendStateCreateInfo"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetLogicOpEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_LOGIC_OP_ENABLE_EXT = 1000455009,

    /// <summary>
    /// Specifies that the blendEnable state in <see cref="VkPipelineColorBlendAttachmentState"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetColorBlendEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_COLOR_BLEND_ENABLE_EXT = 1000455010,

    /// <summary>
    /// Specifies that the srcColorBlendFactor, dstColorBlendFactor, colorBlendOp, srcAlphaBlendFactor, dstAlphaBlendFactor, and alphaBlendOp states in <see cref="VkPipelineColorBlendAttachmentState"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetColorBlendEquation"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_COLOR_BLEND_EQUATION_EXT = 1000455011,

    /// <summary>
    /// Specifies that the colorWriteMask state in <see cref="VkPipelineColorBlendAttachmentState"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetColorWriteMask"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_COLOR_WRITE_MASK_EXT = 1000455012,

    /// <summary>
    /// Specifies that the rasterizationStream state in <see cref="VkPipelineRasterizationStateStreamCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetRasterizationStream"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_RASTERIZATION_STREAM_EXT = 1000455013,

    /// <summary>
    /// Specifies that the conservativeRasterizationMode state in <see cref="VkPipelineRasterizationConservativeStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetConservativeRasterizationMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_CONSERVATIVE_RASTERIZATION_MODE_EXT = 1000455014,

    /// <summary>
    /// Specifies that the extraPrimitiveOverestimationSize state in <see cref="VkPipelineRasterizationConservativeStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetExtraPrimitiveOverestimationSize"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_EXTRA_PRIMITIVE_OVERESTIMATION_SIZE_EXT = 1000455015,

    /// <summary>
    /// Specifies that the depthClipEnable state in <see cref="VkPipelineRasterizationDepthClipStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetDepthClipEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_DEPTH_CLIP_ENABLE_EXT = 1000455016,

    /// <summary>
    /// Specifies that the sampleLocationsEnable state in <see cref="VkPipelineSampleLocationsStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetSampleLocationsEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_SAMPLE_LOCATIONS_ENABLE_EXT = 1000455017,

    /// <summary>
    /// Specifies that the colorBlendOp state in <see cref="VkPipelineColorBlendAttachmentState"/>, and srcPremultiplied, dstPremultiplied, and blendOverlap states in <see cref="VkPipelineColorBlendAdvancedStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetColorBlendAdvanced"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_COLOR_BLEND_ADVANCED_EXT = 1000455018,

    /// <summary>
    /// Specifies that the provokingVertexMode state in <see cref="VkPipelineRasterizationProvokingVertexStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetProvokingVertexMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_PROVOKING_VERTEX_MODE_EXT = 1000455019,

    /// <summary>
    /// Specifies that the lineRasterizationMode state in <see cref="VkPipelineRasterizationLineStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetLineRasterizationMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_LINE_RASTERIZATION_MODE_EXT = 1000455020,

    /// <summary>
    /// Specifies that the stippledLineEnable state in <see cref="VkPipelineRasterizationLineStateCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetLineStippleEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_LINE_STIPPLE_ENABLE_EXT = 1000455021,

    /// <summary>
    /// Specifies that the negativeOneToOne state in <see cref="VkPipelineViewportDepthClipControlCreateInfoEXT"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicState3.CmdSetDepthClipNegativeOneToOne"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_DYNAMIC_STATE_DEPTH_CLIP_NEGATIVE_ONE_TO_ONE_EXT = 1000455022,

    /// <summary>
    /// Specifies that the viewportWScalingEnable state in <see cref="VkPipelineViewportWScalingStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetViewportWScalingEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_clip_space_w_scaling</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_W_SCALING_ENABLE_NV = 1000455023,

    /// <summary>
    /// Specifies that the viewportCount, and pViewportSwizzles states in <see cref="VkPipelineViewportSwizzleStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetViewportSwizzle"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_viewport_swizzle</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_SWIZZLE_NV = 1000455024,

    /// <summary>
    /// Specifies that the coverageToColorEnable state in <see cref="VkPipelineCoverageToColorStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageToColorEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_fragment_coverage_to_color</remarks>
    VK_DYNAMIC_STATE_COVERAGE_TO_COLOR_ENABLE_NV = 1000455025,

    /// <summary>
    /// Specifies that the coverageToColorLocation state in <see cref="VkPipelineCoverageToColorStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageToColorLocation"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_fragment_coverage_to_color</remarks>
    VK_DYNAMIC_STATE_COVERAGE_TO_COLOR_LOCATION_NV = 1000455026,

    /// <summary>
    /// Specifies that the coverageModulationMode state in <see cref="VkPipelineCoverageModulationStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageModulationMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_framebuffer_mixed_samples</remarks>
    VK_DYNAMIC_STATE_COVERAGE_MODULATION_MODE_NV = 1000455027,

    /// <summary>
    /// Specifies that the coverageModulationTableEnable state in <see cref="VkPipelineCoverageModulationStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageModulationTableEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_framebuffer_mixed_samples</remarks>
    VK_DYNAMIC_STATE_COVERAGE_MODULATION_TABLE_ENABLE_NV = 1000455028,

    /// <summary>
    /// Specifies that the coverageModulationTableCount, and pCoverageModulationTable states in <see cref="VkPipelineCoverageModulationStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageModulationTable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_framebuffer_mixed_samples</remarks>
    VK_DYNAMIC_STATE_COVERAGE_MODULATION_TABLE_NV = 1000455029,

    /// <summary>
    /// Specifies that the shadingRateImageEnable state in VkPipelineViewportShadingRateImageStateCreateInfoNV will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetShadingRateImageEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_shading_rate_image</remarks>
    VK_DYNAMIC_STATE_SHADING_RATE_IMAGE_ENABLE_NV = 1000455030,

    /// <summary>
    /// Specifies that the representativeFragmentTestEnable state in <see cref="VkPipelineRepresentativeFragmentTestStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetRepresentativeFragmentTestEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_representative_fragment_test</remarks>
    VK_DYNAMIC_STATE_REPRESENTATIVE_FRAGMENT_TEST_ENABLE_NV = 1000455031,

    /// <summary>
    /// Specifies that the coverageReductionMode state in <see cref="VkPipelineCoverageReductionStateCreateInfoNV"/> will be ignored and must be set dynamically with <see cref="VkExtExtendedDynamicSte3.CmdSetCoverageReductionMode"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_extended_dynamic_state3 with VK_NV_coverage_reduction_mode</remarks>
    VK_DYNAMIC_STATE_COVERAGE_REDUCTION_MODE_NV = 1000455032,

    /// <summary>
    /// Specifies that the VK_PIPELINE_CREATE_COLOR_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT and VK_PIPELINE_CREATE_DEPTH_STENCIL_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT flags will be ignored and must be set dynamically with <see cref="VkExtAttachmentFeedbackLoopDynamicState.CmdSetAttachmentFeedbackLoopEnable"/> before any draw call.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_dynamic_state</remarks>
    VK_DYNAMIC_STATE_ATTACHMENT_FEEDBACK_LOOP_ENABLE_EXT = 1000524000,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_CULL_MODE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_CULL_MODE_EXT = VK_DYNAMIC_STATE_CULL_MODE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_FRONT_FACE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_FRONT_FACE_EXT = VK_DYNAMIC_STATE_FRONT_FACE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_PRIMITIVE_TOPOLOGY" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_PRIMITIVE_TOPOLOGY_EXT = VK_DYNAMIC_STATE_PRIMITIVE_TOPOLOGY,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_VIEWPORT_WITH_COUNT" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_VIEWPORT_WITH_COUNT_EXT = VK_DYNAMIC_STATE_VIEWPORT_WITH_COUNT,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_SCISSOR_WITH_COUNT" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_SCISSOR_WITH_COUNT_EXT = VK_DYNAMIC_STATE_SCISSOR_WITH_COUNT,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_VERTEX_INPUT_BINDING_STRIDE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_VERTEX_INPUT_BINDING_STRIDE_EXT = VK_DYNAMIC_STATE_VERTEX_INPUT_BINDING_STRIDE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_DEPTH_TEST_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_DEPTH_TEST_ENABLE_EXT = VK_DYNAMIC_STATE_DEPTH_TEST_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_DEPTH_WRITE_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_DEPTH_WRITE_ENABLE_EXT = VK_DYNAMIC_STATE_DEPTH_WRITE_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_DEPTH_COMPARE_OP" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_DEPTH_COMPARE_OP_EXT = VK_DYNAMIC_STATE_DEPTH_COMPARE_OP,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_DEPTH_BOUNDS_TEST_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_DEPTH_BOUNDS_TEST_ENABLE_EXT = VK_DYNAMIC_STATE_DEPTH_BOUNDS_TEST_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_STENCIL_TEST_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_STENCIL_TEST_ENABLE_EXT = VK_DYNAMIC_STATE_STENCIL_TEST_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_STENCIL_OP" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_DYNAMIC_STATE_STENCIL_OP_EXT = VK_DYNAMIC_STATE_STENCIL_OP,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE_EXT = VK_DYNAMIC_STATE_RASTERIZER_DISCARD_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_DEPTH_BIAS_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_DYNAMIC_STATE_DEPTH_BIAS_ENABLE_EXT = VK_DYNAMIC_STATE_DEPTH_BIAS_ENABLE,

    /// <inheritdoc cref="VK_DYNAMIC_STATE_PRIMITIVE_RESTART_ENABLE" />
    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_DYNAMIC_STATE_PRIMITIVE_RESTART_ENABLE_EXT = VK_DYNAMIC_STATE_PRIMITIVE_RESTART_ENABLE,
}
