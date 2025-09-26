using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

/// <inheritdoc cref="VkFormat" />
public enum TextureFormat
{
    None              = 0,
    R8Unorm           = VkFormat.R8Unorm,
    R8G8Unorm         = VkFormat.R8G8Unorm,
    B8G8R8A8Unorm     = VkFormat.B8G8R8A8Unorm,
    D32SfloatS8Uint   = VkFormat.D32SfloatS8Uint,
    R16G16B16A16Unorm = VkFormat.R16G16B16A16Unorm,
}
