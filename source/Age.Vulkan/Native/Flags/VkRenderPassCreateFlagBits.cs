namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying additional properties of a render pass.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkRenderPassCreateFlagBits
{
    /// <summary>
    /// Specifies that the created render pass is compatible with <see href="render pass transform">render pass transform</see>.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_render_pass_transform</remarks>
    VK_RENDER_PASS_CREATE_TRANSFORM_BIT_QCOM = 0x00000002,
}
