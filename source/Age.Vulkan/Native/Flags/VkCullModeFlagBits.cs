namespace Age.Vulkan.Native.Flags;

/// <summary>
/// <para>Bitmask controlling triangle culling.</para>
/// <para>Following culling, fragments are produced for any triangles which have not been discarded.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkCullModeFlagBits
{
    /// <summary>
    /// Specifies that no triangles are discarded
    /// </summary>
    VK_CULL_MODE_NONE = 0,

    /// <summary>
    /// Specifies that front-facing triangles are discarded
    /// </summary>
    VK_CULL_MODE_FRONT_BIT = 0x00000001,

    /// <summary>
    /// Specifies that back-facing triangles are discarded
    /// </summary>
    VK_CULL_MODE_BACK_BIT = 0x00000002,

    /// <summary>
    /// Specifies that all triangles are discarded.
    /// </summary>
    VK_CULL_MODE_FRONT_AND_BACK = 0x00000003,
}
