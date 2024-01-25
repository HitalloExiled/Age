namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageLayout.html">VkImageLayout</see>
/// </summary>
public enum ImageLayout
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
    PresentSrcKhr                            = 1000001002,
    VideoDecodeDstKhr                        = 1000024000,
    VideoDecodeSrcKhr                        = 1000024001,
    VideoDecodeDpbKhr                        = 1000024002,
    SharedPresentKhr                         = 1000111000,
    FragmentDensityMapOptimalExt             = 1000218000,
    FragmentShadingRateAttachmentOptimalKhr  = 1000164003,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKhr                        = 1000299000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKhr                        = 1000299001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKhr                        = 1000299002,
#endif
    AttachmentFeedbackLoopOptimalExt         = 1000339000,
    DepthReadOnlyStencilAttachmentOptimalKhr = DepthReadOnlyStencilAttachmentOptimal,
    DepthAttachmentStencilReadOnlyOptimalKhr = DepthAttachmentStencilReadOnlyOptimal,
    ShadingRateOptimalNv                     = FragmentShadingRateAttachmentOptimalKhr,
    DepthAttachmentOptimalKhr                = DepthAttachmentOptimal,
    DepthReadOnlyOptimalKhr                  = DepthReadOnlyOptimal,
    StencilAttachmentOptimalKhr              = StencilAttachmentOptimal,
    StencilReadOnlyOptimalKhr                = StencilReadOnlyOptimal,
    ReadOnlyOptimalKhr                       = ReadOnlyOptimal,
    AttachmentOptimalKhr                     = AttachmentOptimal,
}
