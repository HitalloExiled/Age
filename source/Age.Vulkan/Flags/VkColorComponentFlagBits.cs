namespace Age.Vulkan.Flags;

/// <summary>
/// <para>Bitmask controlling which components are written to the framebuffer.</para>
/// <para>Bits which can be set in <see cref="VkPipelineColorBlendAttachmentState.colorWriteMask"/>, determining whether the final color values R, G, B and A are written to the framebuffer attachment.</para>
/// <para>The color write mask operation is applied regardless of whether blending is enabled.</para>
/// <para>The color write mask operation is applied only if Color Write Enable is enabled for the respective attachment. Otherwise the color write mask is ignored and writes to all components of the attachment are disabled.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkColorComponentFlagBits
{
    /// <summary>
    /// Specifies that the R value is written to the color attachment for the appropriate sample. Otherwise, the value in memory is unmodified.
    /// </summary>
    VK_COLOR_COMPONENT_R_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the G value is written to the color attachment for the appropriate sample. Otherwise, the value in memory is unmodified.
    /// </summary>
    VK_COLOR_COMPONENT_G_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the B value is written to the color attachment for the appropriate sample. Otherwise, the value in memory is unmodified.
    /// </summary>
    VK_COLOR_COMPONENT_B_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the A value is written to the color attachment for the appropriate sample. Otherwise, the value in memory is unmodified.
    /// </summary>
    VK_COLOR_COMPONENT_A_BIT = 0x00000008,
}
