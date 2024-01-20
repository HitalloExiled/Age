namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying usage behavior for command buffer.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkCommandBufferUsageFlagBits
{
    /// <summary>
    /// Specifies that each recording of the command buffer will only be submitted once, and the command buffer will be reset and recorded again between each submission.
    /// </summary>
    VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT = 0x00000001,

    /// <summary>
    /// Specifies that a secondary command buffer is considered to be entirely inside a render pass. If this is a primary command buffer, then this bit is ignored.
    /// </summary>
    VK_COMMAND_BUFFER_USAGE_RENDER_PASS_CONTINUE_BIT = 0x00000002,

    /// <summary>
    /// Specifies that a command buffer can be resubmitted to a queue while it is in the pending state, and recorded into multiple primary command buffers.
    /// </summary>
    VK_COMMAND_BUFFER_USAGE_SIMULTANEOUS_USE_BIT = 0x00000004,
}
