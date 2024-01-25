namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageUsageFlagBits.html">VkImageUsageFlagBits</see>
/// </summary>
[Flags]
public enum ImageUsageFlags
{
    TransferSrc                         = 0x00000001,
    TransferDst                         = 0x00000002,
    Sampled                             = 0x00000004,
    Storage                             = 0x00000008,
    ColorAttachment                     = 0x00000010,
    DepthStencilAttachment           = 0x00000020,
    TransientAttachment              = 0x00000040,
    InputAttachment                  = 0x00000080,
    VideoDecodeDstKhr                = 0x00000400,
    VideoDecodeSrcKhr                = 0x00000800,
    VideoDecodeDpbKhr                = 0x00001000,
    FragmentDensityMapExt            = 0x00000200,
    FragmentShadingRateAttachmentKhr = 0x00000100,
    HostTransferExt                  = 0x00400000,

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKhr                = 0x00002000,
#endif

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKhr                = 0x00004000,
#endif

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKhr                = 0x00008000,
#endif

    AttachmentFeedbackLoopExt        = 0x00080000,
    InvocationMaskHuawei             = 0x00040000,
    SampleWeightQcom                 = 0x00100000,
    SampleBlockMatchQcom             = 0x00200000,
    ShadingRateImageNv               = FragmentShadingRateAttachmentKhr,
}
