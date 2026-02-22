using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

/// <inheritdoc cref="VkImageUsageFlags" />
[Flags]
public enum TextureUsage
{
    None                   = 0,
    TransferSrc            = VkImageUsageFlags.TransferSrc,
    TransferDst            = VkImageUsageFlags.TransferDst,
    Sampled                = VkImageUsageFlags.Sampled,
    ColorAttachment        = VkImageUsageFlags.ColorAttachment,
    DepthStencilAttachment = VkImageUsageFlags.DepthStencilAttachment,
}
