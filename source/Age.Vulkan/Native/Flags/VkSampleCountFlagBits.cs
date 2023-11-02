namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying sample counts supported for an image used for storage operations.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkSampleCountFlagBits
{
    /// <summary>
    /// Specifies an image with one sample per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_1_BIT = 0x00000001,

    /// <summary>
    /// Specifies an image with 2 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_2_BIT = 0x00000002,

    /// <summary>
    /// Specifies an image with 4 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_4_BIT = 0x00000004,

    /// <summary>
    /// Specifies an image with 8 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_8_BIT = 0x00000008,

    /// <summary>
    /// Specifies an image with 16 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_16_BIT = 0x00000010,

    /// <summary>
    /// Specifies an image with 32 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_32_BIT = 0x00000020,

    /// <summary>
    /// Specifies an image with 64 samples per pixel.
    /// </summary>
    VK_SAMPLE_COUNT_64_BIT = 0x00000040,
}
