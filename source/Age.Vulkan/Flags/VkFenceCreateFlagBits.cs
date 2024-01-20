namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying initial state and behavior of a fence.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkFenceCreateFlagBits
{
    /// <summary>
    /// Specifies that the fence object is created in the signaled state. Otherwise, it is created in the unsignaled state.
    /// </summary>
    VK_FENCE_CREATE_SIGNALED_BIT = 0x00000001,
}
