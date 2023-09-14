namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Bitmask specifying which types of events cause a debug messenger callback
/// </summary>
[Flags]
public enum VkDebugUtilsMessageTypeFlagBitsEXT : uint
{
    /// <summary>
    /// Specifies that some general event has occurred. This is typically a non-specification, non-performance event.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT = 0x00000001,

    /// <summary>
    /// Specifies that something has occurred during validation against the Vulkan specification that may indicate invalid behavior.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT = 0x00000002,

    /// <summary>
    /// Specifies a potentially non-optimal use of Vulkan, e.g. using <see cref="VK.CmdClearColorImage"/> when setting <see cref="VkAttachmentDescription.loadOp"/> to <see cref="VK_ATTACHMENT_LOAD_OP_CLEAR"/> would have worked.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT = 0x00000004,

    /// <summary>
    /// Specifies that the implementation has modified the set of GPU-visible virtual addresses associated with a Vulkan object.
    /// </summary>
    /// <remarks>Provided by VK_EXT_device_address_binding_report</remarks>
    VK_DEBUG_UTILS_MESSAGE_TYPE_DEVICE_ADDRESS_BINDING_BIT_EXT = 0x00000008,
}
