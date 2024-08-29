namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageUsageFlagBits.html">VkImageUsageFlagBits</see>
/// </summary>
[Flags]
public enum VkImageUsageFlags
{
    TransferSrc                         = 0x00000001,
    TransferDst                         = 0x00000002,
    Sampled                             = 0x00000004,
    Storage                             = 0x00000008,
    ColorAttachment                     = 0x00000010,
    DepthStencilAttachment           = 0x00000020,
    TransientAttachment              = 0x00000040,
    InputAttachment                  = 0x00000080,
    VideoDecodeDstKHR                = 0x00000400,
    VideoDecodeSrcKHR                = 0x00000800,
    VideoDecodeDpbKHR                = 0x00001000,
    FragmentDensityMapEXT            = 0x00000200,
    FragmentShadingRateAttachmentKHR = 0x00000100,
    HostTransferEXT                  = 0x00400000,

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKHR                = 0x00002000,
#endif

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKHR                = 0x00004000,
#endif

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKHR                = 0x00008000,
#endif

    AttachmentFeedbackLoopEXT        = 0x00080000,
    InvocationMaskHUAWEI             = 0x00040000,
    SampleWeightQCOM                 = 0x00100000,
    SampleBlockMatchQCOM             = 0x00200000,
    ShadingRateImageNV               = FragmentShadingRateAttachmentKHR,
}
