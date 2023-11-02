using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying an image subresource layers.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkImageSubresourceLayers
{
    /// <summary>
    /// A combination of VkImageAspectFlagBits, selecting the color, depth and/or stencil aspects to be copied.
    /// </summary>
    public VkImageAspectFlags aspectMask;

    /// <summary>
    /// The mipmap level to copy
    /// </summary>
    public uint mipLevel;

    /// <summary>
    /// <see cref="baseArrayLayer"/> and <see cref="layerCount"/> are the starting layer and number of layers to copy.
    /// </summary>
    public uint baseArrayLayer;

    /// <inheritdoc cref="baseArrayLayer" />
    public uint layerCount;
}
