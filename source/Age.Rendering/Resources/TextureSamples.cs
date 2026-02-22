using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

[Flags]
public enum TextureSamples
{
    None = 0,
    N1   = VkSampleCountFlags.N1,
    N2   = VkSampleCountFlags.N2,
    N4   = VkSampleCountFlags.N4,
    N8   = VkSampleCountFlags.N8,
    N16  = VkSampleCountFlags.N16,
    N32  = VkSampleCountFlags.N32,
    N64  = VkSampleCountFlags.N64,
}
