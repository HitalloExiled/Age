namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFormatFeatureFlagBits.html">VkFormatFeatureFlagBits</see>
/// </summary>
[Flags]
public enum VkFormatFeatureFlags
{
    SampledImage                                                        = 0x00000001,
    StorageImage                                                        = 0x00000002,
    StorageImageAtomic                                                  = 0x00000004,
    UniformTexelBuffer                                                  = 0x00000008,
    StorageTexelBuffer                                                  = 0x00000010,
    StorageTexelBufferAtomic                                            = 0x00000020,
    VertexBuffer                                                        = 0x00000040,
    ColorAttachment                                                     = 0x00000080,
    ColorAttachmentBlend                                                = 0x00000100,
    DepthStencilAttachment                                              = 0x00000200,
    BlitSrc                                                             = 0x00000400,
    BlitDst                                                             = 0x00000800,
    SampledImageFilterLinear                                            = 0x00001000,
    TransferSrc                                                         = 0x00004000,
    TransferDst                                                         = 0x00008000,
    MidpointChromaSamples                                               = 0x00020000,
    SampledImageYcbcrConversionLinearFilter                             = 0x00040000,
    SampledImageYcbcrConversionSeparateReconstructionFilter             = 0x00080000,
    SampledImageYcbcrConversionChromaReconstructionExplicit             = 0x00100000,
    SampledImageYcbcrConversionChromaReconstructionExplicitForceable    = 0x00200000,
    Disjoint                                                            = 0x00400000,
    CositedChromaSamples                                                = 0x00800000,
    SampledImageFilterMinmax                                            = 0x00010000,
    VideoDecodeOutputKHR                                                = 0x02000000,
    VideoDecodeDpbKHR                                                   = 0x04000000,
    AccelerationStructureVertexBufferKHR                                = 0x20000000,
    SampledImageFilterCubicEXT                                          = 0x00002000,
    FragmentDensityMapEXT                                               = 0x01000000,
    FragmentShadingRateAttachmentKHR                                    = 0x40000000,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeInputKHR                                                 = 0x08000000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKHR                                                   = 0x10000000,
#endif
    SampledImageFilterCubicImg                                          = SampledImageFilterCubicEXT,
    TransferSrcKHR                                                      = TransferSrc,
    TransferDstKHR                                                      = TransferDst,
    SampledImageFilterMinmaxEXT                                         = SampledImageFilterMinmax,
    MidpointChromaSamplesKHR                                            = MidpointChromaSamples,
    SampledImageYcbcrConversionLinearFilterKHR                          = SampledImageYcbcrConversionLinearFilter,
    SampledImageYcbcrConversionSeparateReconstructionFilterKHR          = SampledImageYcbcrConversionSeparateReconstructionFilter,
    SampledImageYcbcrConversionChromaReconstructionExplicitKHR          = SampledImageYcbcrConversionChromaReconstructionExplicit,
    SampledImageYcbcrConversionChromaReconstructionExplicitForceableKHR = SampledImageYcbcrConversionChromaReconstructionExplicitForceable,
    DisjointKHR                                                         = Disjoint,
    CositedChromaSamplesKHR                                             = CositedChromaSamples,
}
