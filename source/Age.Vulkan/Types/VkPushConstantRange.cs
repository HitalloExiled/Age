using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a push constant range.
/// </summary>
public struct VkPushConstantRange
{
    /// <summary>
    /// A set of stage flags describing the shader stages that will access a range of push constants. If a particular stage is not included in the range, then accessing members of that range of push constants from the corresponding shader stage will return undefined values.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkShaderStageFlags stageFlags;

    /// <summary>
    /// <see cref="offset"/> and <see cref="size"/> are the start offset and size, respectively, consumed by the range. Both offset and size are in units of bytes and must be a multiple of 4. The layout of the push constant variables is specified in the shader.
    /// </summary>
    public uint offset;

    /// <inheritdoc cref="offset" />
    public uint size;
}
