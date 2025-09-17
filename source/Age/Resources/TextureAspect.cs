using ThirdParty.Vulkan.Flags;

namespace Age.Resources;

[Flags]
public enum TextureAspect
{
    None    = 0,
    Color   = VkImageAspectFlags.Color,
    Depth   = VkImageAspectFlags.Depth,
    Stencil = VkImageAspectFlags.Stencil,
}
