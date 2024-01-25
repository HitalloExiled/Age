namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineCreateFlagBits.html">VkPipelineCreateFlagBits</see>
/// </summary>
[Flags]
public enum PipelineCreateFlags
{
    DisableOptimization                         = 0x00000001,
    AllowDerivatives                            = 0x00000002,
    Derivative                                  = 0x00000004,
    ViewIndexFromDeviceIndex                    = 0x00000008,
    DispatchBase                                = 0x00000010,
    FailOnPipelineCompileRequired               = 0x00000100,
    EarlyReturnOnFailure                        = 0x00000200,
    RenderingFragmentShadingRateAttachmentKhr   = 0x00200000,
    RenderingFragmentDensityMapAttachmentExt    = 0x00400000,
    RayTracingNoNullAnyHitShadersKhr            = 0x00004000,
    RayTracingNoNullClosestHitShadersKhr        = 0x00008000,
    RayTracingNoNullMissShadersKhr              = 0x00010000,
    RayTracingNoNullIntersectionShadersKhr      = 0x00020000,
    RayTracingSkipTrianglesKhr                  = 0x00001000,
    RayTracingSkipAabbsKhr                      = 0x00002000,
    RayTracingShaderGroupHandleCaptureReplayKhr = 0x00080000,
    DeferCompileNv                              = 0x00000020,
    CaptureStatisticsKhr                        = 0x00000040,
    CaptureInternalRepresentationsKhr           = 0x00000080,
    IndirectBindableNv                          = 0x00040000,
    LibraryKhr                                  = 0x00000800,
    DescriptorBufferExt                         = 0x20000000,
    RetainLinkTimeOptimizationInfoExt           = 0x00800000,
    LinkTimeOptimizationExt                     = 0x00000400,
    RayTracingAllowMotionNv                     = 0x00100000,
    ColorAttachmentFeedbackLoopExt              = 0x02000000,
    DepthStencilAttachmentFeedbackLoopExt       = 0x04000000,
    RayTracingOpacityMicromapExt                = 0x01000000,

#if VK_ENABLE_BETA_EXTENSIONS
    RayTracingDisplacementMicromapNv            = 0x10000000,
#endif

    NoProtectedAccessExt                        = 0x08000000,
    ProtectedAccessOnlyExt                      = 0x40000000,
    ShadingRateAttachmentKhr                    = RenderingFragmentShadingRateAttachmentKhr,
    DensityMapAttachmentExt                     = RenderingFragmentDensityMapAttachmentExt,
    ViewIndexFromDeviceIndexKhr                 = ViewIndexFromDeviceIndex,
    FailOnPipelineCompileRequiredExt            = FailOnPipelineCompileRequired,
    EarlyReturnOnFailureExt                     = EarlyReturnOnFailure,
}
