namespace Age.Vulkan.Enums;

/// <summary>
/// Specify mipmap mode used for texture lookups.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkSamplerMipmapMode
{
    /// <summary>
    /// Specifies nearest filtering.
    /// </summary>
    VK_SAMPLER_MIPMAP_MODE_NEAREST = 0,

    /// <summary>
    /// Specifies linear filtering.
    /// </summary>
    VK_SAMPLER_MIPMAP_MODE_LINEAR = 1,
}
