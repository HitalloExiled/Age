namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDynamicState.html">VkDynamicState</see>
/// </summary>
public enum VkDynamicState
{
    Viewport                            = 0,
    Scissor                             = 1,
    LineWidth                           = 2,
    DepthBias                           = 3,
    BlendConstants                      = 4,
    DepthBounds                         = 5,
    StencilCompareMask                  = 6,
    StencilWriteMask                    = 7,
    StencilReference                    = 8,
    CullMode                            = 1000267000,
    FrontFace                           = 1000267001,
    PrimitiveTopology                   = 1000267002,
    ViewportWithCount                   = 1000267003,
    ScissorWithCount                    = 1000267004,
    VertexInputBindingStride            = 1000267005,
    DepthTestEnable                     = 1000267006,
    DepthWriteEnable                    = 1000267007,
    DepthCompareOp                      = 1000267008,
    DepthBoundsTestEnable               = 1000267009,
    StencilTestEnable                   = 1000267010,
    StencilOp                           = 1000267011,
    RasterizerDiscardEnable             = 1000377001,
    DepthBiasEnable                     = 1000377002,
    PrimitiveRestartEnable              = 1000377004,
    ViewportWScalingNV                  = 1000087000,
    DiscardRectangleEXT                 = 1000099000,
    DiscardRectangleEnableEXT           = 1000099001,
    DiscardRectangleModeEXT             = 1000099002,
    SampleLocationsEXT                  = 1000143000,
    RayTracingPipelineStackSizeKHR      = 1000347000,
    ViewportShadingRatePaletteNV        = 1000164004,
    ViewportCoarseSampleOrderNV         = 1000164006,
    ExclusiveScissorEnableNV            = 1000205000,
    ExclusiveScissorNV                  = 1000205001,
    FragmentShadingRateKHR              = 1000226000,
    LineStippleEXT                      = 1000259000,
    VertexInputEXT                      = 1000352000,
    PatchControlPointsEXT               = 1000377000,
    LogicOpEXT                          = 1000377003,
    ColorWriteEnableEXT                 = 1000381000,
    TessellationDomainOriginEXT         = 1000455002,
    DepthClampEnableEXT                 = 1000455003,
    PolygonModeEXT                      = 1000455004,
    RasterizationSamplesEXT             = 1000455005,
    SampleMaskEXT                       = 1000455006,
    AlphaToCoverageEnableEXT            = 1000455007,
    AlphaToOneEnableEXT                 = 1000455008,
    LogicOpEnableEXT                    = 1000455009,
    ColorBlendEnableEXT                 = 1000455010,
    ColorBlendEquationEXT               = 1000455011,
    ColorWriteMaskEXT                   = 1000455012,
    RasterizationStreamEXT              = 1000455013,
    ConservativeRasterizationModeEXT    = 1000455014,
    ExtraPrimitiveOverestimationSizeEXT = 1000455015,
    DepthClipEnableEXT                  = 1000455016,
    SampleLocationsEnableEXT            = 1000455017,
    ColorBlendAdvancedEXT               = 1000455018,
    ProvokingVertexModeEXT              = 1000455019,
    LineRasterizationModeEXT            = 1000455020,
    LineStippleEnableEXT                = 1000455021,
    DepthClipNegativeOneToOneEXT        = 1000455022,
    ViewportWScalingEnableNV            = 1000455023,
    ViewportSwizzleNV                   = 1000455024,
    CoverageToColorEnableNV             = 1000455025,
    CoverageToColorLocationNV           = 1000455026,
    CoverageModulationModeNV            = 1000455027,
    CoverageModulationTableEnableNV     = 1000455028,
    CoverageModulationTableNV           = 1000455029,
    ShadingRateImageEnableNV            = 1000455030,
    RepresentativeFragmentTestEnableNV  = 1000455031,
    CoverageReductionModeNV             = 1000455032,
    AttachmentFeedbackLoopEnableEXT     = 1000524000,
    CullModeEXT                         = CullMode,
    FrontFaceEXT                        = FrontFace,
    PrimitiveTopologyEXT                = PrimitiveTopology,
    ViewportWithCountEXT                = ViewportWithCount,
    ScissorWithCountEXT                 = ScissorWithCount,
    VertexInputBindingStrideEXT         = VertexInputBindingStride,
    DepthTestEnableEXT                  = DepthTestEnable,
    DepthWriteEnableEXT                 = DepthWriteEnable,
    DepthCompareOpEXT                   = DepthCompareOp,
    DepthBoundsTestEnableEXT            = DepthBoundsTestEnable,
    StencilTestEnableEXT                = StencilTestEnable,
    StencilOpEXT                        = StencilOp,
    RasterizerDiscardEnableEXT          = RasterizerDiscardEnable,
    DepthBiasEnableEXT                  = DepthBiasEnable,
    PrimitiveRestartEnableEXT           = PrimitiveRestartEnable,
}