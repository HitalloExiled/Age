namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying usage behavior for a command pool
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkCommandPoolCreateFlagBits
{
    /// <summary>
    /// Specifies that command buffers allocated from the pool will be short-lived, meaning that they will be reset or freed in a relatively short timeframe. This flag may be used by the implementation to control memory allocation behavior within the pool.
    /// </summary>
    VK_COMMAND_POOL_CREATE_TRANSIENT_BIT = 0x00000001,

    /// <summary>
    /// Allows any command buffer allocated from a pool to be individually reset to the initial state; either by calling vkResetCommandBuffer, or via the implicit reset when calling vkBeginCommandBuffer. If this flag is not set on a pool, then vkResetCommandBuffer must not be called for any command buffer allocated from that pool.
    /// </summary>
    VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT = 0x00000002,

    /// <summary>
    /// Specifies that command buffers allocated from the pool are protected command buffers.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_COMMAND_POOL_CREATE_PROTECTED_BIT = 0x00000004,
}
