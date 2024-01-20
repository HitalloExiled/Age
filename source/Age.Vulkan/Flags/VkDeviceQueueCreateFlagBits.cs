namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying behavior of the queue.
/// </summary>
[Flags]
public enum VkDeviceQueueCreateFlagBits
{
    /// <summary>
    /// Specifies that the device queue is a protected-capable queue.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_DEVICE_QUEUE_CREATE_PROTECTED_BIT = 0x00000001,
}
