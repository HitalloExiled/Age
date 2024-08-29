namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineCreateFlagBits.html">VkPipelineCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkPipelineCreateFlags
{
    DisableOptimization                         = 0x00000001,
    AllowDerivatives                            = 0x00000002,
    Derivative                                  = 0x00000004,
    ViewIndexFromDeviceIndex                    = 0x00000008,
    DispatchBase                                = 0x00000010,
    FailOnPipelineCompileRequired               = 0x00000100,
    EarlyReturnOnFailure                        = 0x00000200,
    RenderingFragmentShadingRateAttachmentKHR   = 0x00200000,
    RenderingFragmentDensityMapAttachmentEXT    = 0x00400000,
    RayTracingNoNullAnyHitShadersKHR            = 0x00004000,
    RayTracingNoNullClosestHitShadersKHR        = 0x00008000,
    RayTracingNoNullMissShadersKHR              = 0x00010000,
    RayTracingNoNullIntersectionShadersKHR      = 0x00020000,
    RayTracingSkipTrianglesKHR                  = 0x00001000,
    RayTracingSkipAabbsKHR                      = 0x00002000,
    RayTracingShaderGroupHandleCaptureReplayKHR = 0x00080000,
    DeferCompileNV                              = 0x00000020,
    CaptureStatisticsKHR                        = 0x00000040,
    CaptureInternalRepresentationsKHR           = 0x00000080,
    IndirectBindableNV                          = 0x00040000,
    LibraryKHR                                  = 0x00000800,
    DescriptorBufferEXT                         = 0x20000000,
    RetainLinkTimeOptimizationInfoEXT           = 0x00800000,
    LinkTimeOptimizationEXT                     = 0x00000400,
    RayTracingAllowMotionNV                     = 0x00100000,
    ColorAttachmentFeedbackLoopEXT              = 0x02000000,
    DepthStencilAttachmentFeedbackLoopEXT       = 0x04000000,
    RayTracingOpacityMicromapEXT                = 0x01000000,

#if VK_ENABLE_BETA_EXTENSIONS
    RayTracingDisplacementMicromapNV            = 0x10000000,
#endif

    NoProtectedAccessEXT                        = 0x08000000,
    ProtectedAccessOnlyEXT                      = 0x40000000,
    ShadingRateAttachmentKHR                    = RenderingFragmentShadingRateAttachmentKHR,
    DensityMapAttachmentEXT                     = RenderingFragmentDensityMapAttachmentEXT,
    ViewIndexFromDeviceIndexKHR                 = ViewIndexFromDeviceIndex,
    FailOnPipelineCompileRequiredEXT            = FailOnPipelineCompileRequired,
    EarlyReturnOnFailureEXT                     = EarlyReturnOnFailure,
}
