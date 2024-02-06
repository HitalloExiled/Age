namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageLayout.html">VkImageLayout</see>
/// </summary>
public enum VkImageLayout
{
    Undefined                                = 0,
    General                                  = 1,
    ColorAttachmentOptimal                   = 2,
    DepthStencilAttachmentOptimal            = 3,
    DepthStencilReadOnlyOptimal              = 4,
    ShaderReadOnlyOptimal                    = 5,
    TransferSrcOptimal                       = 6,
    TransferDstOptimal                       = 7,
    Preinitialized                           = 8,
    DepthReadOnlyStencilAttachmentOptimal    = 1000117000,
    DepthAttachmentStencilReadOnlyOptimal    = 1000117001,
    DepthAttachmentOptimal                   = 1000241000,
    DepthReadOnlyOptimal                     = 1000241001,
    StencilAttachmentOptimal                 = 1000241002,
    StencilReadOnlyOptimal                   = 1000241003,
    ReadOnlyOptimal                          = 1000314000,
    AttachmentOptimal                        = 1000314001,
    PresentSrcKHR                            = 1000001002,
    VideoDecodeDstKHR                        = 1000024000,
    VideoDecodeSrcKHR                        = 1000024001,
    VideoDecodeDpbKHR                        = 1000024002,
    SharedPresentKHR                         = 1000111000,
    FragmentDensityMapOptimalEXT             = 1000218000,
    FragmentShadingRateAttachmentOptimalKHR  = 1000164003,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKHR                        = 1000299000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKHR                        = 1000299001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKHR                        = 1000299002,
#endif
    AttachmentFeedbackLoopOptimalEXT         = 1000339000,
    DepthReadOnlyStencilAttachmentOptimalKHR = DepthReadOnlyStencilAttachmentOptimal,
    DepthAttachmentStencilReadOnlyOptimalKHR = DepthAttachmentStencilReadOnlyOptimal,
    ShadingRateOptimalNV                     = FragmentShadingRateAttachmentOptimalKHR,
    DepthAttachmentOptimalKHR                = DepthAttachmentOptimal,
    DepthReadOnlyOptimalKHR                  = DepthReadOnlyOptimal,
    StencilAttachmentOptimalKHR              = StencilAttachmentOptimal,
    StencilReadOnlyOptimalKHR                = StencilReadOnlyOptimal,
    ReadOnlyOptimalKHR                       = ReadOnlyOptimal,
    AttachmentOptimalKHR                     = AttachmentOptimal,
}
