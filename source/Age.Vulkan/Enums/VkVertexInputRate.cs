namespace Age.Vulkan.Enums;

/// <summary>
/// Specify rate at which vertex attributes are pulled from buffers.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkVertexInputRate
{
    /// <summary>
    /// Specifies that vertex attribute addressing is a function of the vertex index.
    /// </summary>
    VK_VERTEX_INPUT_RATE_VERTEX = 0,

    /// <summary>
    /// Specifies that vertex attribute addressing is a function of the instance index.
    /// </summary>
    VK_VERTEX_INPUT_RATE_INSTANCE = 1,
}
