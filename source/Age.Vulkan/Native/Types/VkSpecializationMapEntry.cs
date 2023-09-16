namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying a specialization map entry.</para>
/// <para>If a constantID value is not a specialization constant ID used in the shader, that map entry does not affect the behavior of the pipeline.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkSpecializationMapEntry
{
    /// <summary>
    /// The ID of the specialization constant in SPIR-V.
    /// </summary>
    public uint constantID;

    /// <summary>
    /// The byte offset of the specialization constant value within the supplied data buffer.
    /// </summary>
    public uint offset;

    /// <summary>
    /// The byte size of the specialization constant value within the supplied data buffer.
    /// </summary>
    public uint size;
}
