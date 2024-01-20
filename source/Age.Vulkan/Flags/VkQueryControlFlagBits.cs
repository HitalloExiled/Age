namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying constraints on a query.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkQueryControlFlagBits
{
    /// <summary>
    /// Specifies the precision of <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#queries-occlusion">occlusion queries</see>.
    /// </summary>
    VK_QUERY_CONTROL_PRECISE_BIT = 0x00000001,
}
