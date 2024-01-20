namespace Age.Vulkan.Enums;

/// <summary>
/// Type of index buffer indices.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkIndexType
{
    /// <summary>
    /// Specifies that indices are 16-bit unsigned integer values.
    /// </summary>
    VK_INDEX_TYPE_UINT16 = 0,

    /// <summary>
    /// Specifies that indices are 32-bit unsigned integer values.
    /// </summary>
    VK_INDEX_TYPE_UINT32 = 1,

    /// <summary>
    /// Specifies that no indices are provided.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_INDEX_TYPE_NONE_KHR = 1000165000,

    /// <summary>
    /// Specifies that indices are 8-bit unsigned integer values.
    /// </summary>
    /// <remarks>Provided by VK_EXT_index_type_uint8</remarks>
    VK_INDEX_TYPE_UINT8_EXT = 1000265000,

    /// <inheritdoc cref="VK_INDEX_TYPE_NONE_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_INDEX_TYPE_NONE_NV = VK_INDEX_TYPE_NONE_KHR,
}
