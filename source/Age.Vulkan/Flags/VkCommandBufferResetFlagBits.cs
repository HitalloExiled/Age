namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask controlling behavior of a command buffer reset.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkCommandBufferResetFlagBits
{
    /// <summary>
    /// Specifies that most or all memory resources currently owned by the command buffer should be returned to the parent command pool. If this flag is not set, then the command buffer may hold onto memory resources and reuse them when recording commands. commandBuffer is moved to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">initial state</see>.
    /// </summary>
    VK_COMMAND_BUFFER_RESET_RELEASE_RESOURCES_BIT = 0x00000001,
}
