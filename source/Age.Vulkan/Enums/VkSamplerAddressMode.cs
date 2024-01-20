namespace Age.Vulkan.Enums;

/// <summary>
/// Specify behavior of sampling with texture coordinates outside an image.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkSamplerAddressMode
{
    /// <summary>
    /// Specifies that the repeat wrap mode will be used.
    /// </summary>
    VK_SAMPLER_ADDRESS_MODE_REPEAT = 0,

    /// <summary>
    /// Specifies that the mirrored repeat wrap mode will be used.
    /// </summary>
    VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT = 1,

    /// <summary>
    /// Specifies that the clamp to edge wrap mode will be used.
    /// </summary>
    VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE = 2,

    /// <summary>
    /// Specifies that the clamp to border wrap mode will be used.
    /// </summary>
    VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER = 3,

    /// <summary>
    /// Specifies that the mirror clamp to edge wrap mode will be used. This is only valid if <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-samplerMirrorClampToEdge">samplerMirrorClampToEdge</see> is enabled, or if the <see cref="VkKhrSamplerMirrorClampToEdge"/> extension is enabled.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2, VK_KHR_sampler_mirror_clamp_to_edge</remarks>
    VK_SAMPLER_ADDRESS_MODE_MIRROR_CLAMP_TO_EDGE = 4,

    /// <inheritdoc cref="VK_SAMPLER_ADDRESS_MODE_MIRROR_CLAMP_TO_EDGE" />
    /// <remarks>Provided by VK_KHR_sampler_mirror_clamp_to_edge</remarks>
    VK_SAMPLER_ADDRESS_MODE_MIRROR_CLAMP_TO_EDGE_KHR = VK_SAMPLER_ADDRESS_MODE_MIRROR_CLAMP_TO_EDGE,
}
