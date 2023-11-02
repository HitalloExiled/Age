namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Specify how commands in the first subpass of a render pass are provided.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkSubpassContents
{
    /// <summary>
    /// Specifies that the contents of the subpass will be recorded inline in the primary command buffer, and secondary command buffers must not be executed within the subpass.
    /// </summary>
    VK_SUBPASS_CONTENTS_INLINE = 0,

    /// <summary>
    /// Specifies that the contents are recorded in secondary command buffers that will be called from the primary command buffer, and <see cref="Vk.CmdExecuteCommands"/> is the only valid command in the command buffer until <see cref="Vk.CmdNextSubpass"/> or <see cref="Vk.CmdEndRenderPass"/>.
    /// </summary>
    VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS = 1,
}
