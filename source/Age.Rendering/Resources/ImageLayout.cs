using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

/// <inheritdoc cref="VkImageLayout" />
public enum ImageLayout
{
    Undefined                                = VkImageLayout.Undefined,
    General                                  = VkImageLayout.General,
    ColorAttachmentOptimal                   = VkImageLayout.ColorAttachmentOptimal,
    DepthStencilAttachmentOptimal            = VkImageLayout.DepthStencilAttachmentOptimal,
    DepthStencilReadOnlyOptimal              = VkImageLayout.DepthStencilReadOnlyOptimal,
    ShaderReadOnlyOptimal                    = VkImageLayout.ShaderReadOnlyOptimal,
    TransferSrcOptimal                       = VkImageLayout.TransferSrcOptimal,
    TransferDstOptimal                       = VkImageLayout.TransferDstOptimal,
    Preinitialized                           = VkImageLayout.Preinitialized,
    DepthReadOnlyStencilAttachmentOptimal    = VkImageLayout.DepthReadOnlyStencilAttachmentOptimal,
    DepthAttachmentStencilReadOnlyOptimal    = VkImageLayout.DepthAttachmentStencilReadOnlyOptimal,
    DepthAttachmentOptimal                   = VkImageLayout.DepthAttachmentOptimal,
    DepthReadOnlyOptimal                     = VkImageLayout.DepthReadOnlyOptimal,
    StencilAttachmentOptimal                 = VkImageLayout.StencilAttachmentOptimal,
    StencilReadOnlyOptimal                   = VkImageLayout.StencilReadOnlyOptimal,
    ReadOnlyOptimal                          = VkImageLayout.ReadOnlyOptimal,
    AttachmentOptimal                        = VkImageLayout.AttachmentOptimal,
    PresentSrcKHR                            = VkImageLayout.PresentSrcKHR,
    VideoDecodeDstKHR                        = VkImageLayout.VideoDecodeDstKHR,
    VideoDecodeSrcKHR                        = VkImageLayout.VideoDecodeSrcKHR,
    VideoDecodeDpbKHR                        = VkImageLayout.VideoDecodeDpbKHR,
    SharedPresentKHR                         = VkImageLayout.SharedPresentKHR,
    FragmentDensityMapOptimalEXT             = VkImageLayout.FragmentDensityMapOptimalEXT,
    FragmentShadingRateAttachmentOptimalKHR  = VkImageLayout.FragmentShadingRateAttachmentOptimalKHR,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKHR                        = VkImageLayout.VideoEncodeDstKHR,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKHR                        = VkImageLayout.VideoEncodeSrcKHR,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDpbKHR                        = VkImageLayout.VideoEncodeDpbKHR,
#endif
    AttachmentFeedbackLoopOptimalEXT         = VkImageLayout.AttachmentFeedbackLoopOptimalEXT,
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
