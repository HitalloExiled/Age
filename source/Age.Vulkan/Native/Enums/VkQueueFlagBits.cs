namespace Age.Vulkan.Native.Enums;

/// <summary>
/// <para>Bitmask specifying capabilities of queues in a queue family</para>
/// <para>If an implementation exposes any queue family that supports graphics operations, at least one queue family of at least one physical device exposed by the implementation must support both graphics and compute operations.</para>
/// <para>Furthermore, if the protectedMemory physical device feature is supported, then at least one queue family of at least one physical device exposed by the implementation must support graphics operations, compute operations, and protected memory operations.</para>
/// <remarks>All commands that are allowed on a queue that supports transfer operations are also allowed on a queue that supports either graphics or compute operations. Thus, if the capabilities of a queue family include <see cref="VK_QUEUE_GRAPHICS_BIT"/> or <see cref="VK_QUEUE_COMPUTE_BIT"/>, then reporting the <see cref="VK_QUEUE_TRANSFER_BIT"/> capability separately for that queue family is optional.</remarks>
/// </summary>
public enum VkQueueFlagBits : uint
{
    /// <summary>
    /// Specifies that queues in this queue family support graphics operations.
    /// </summary>
    VK_QUEUE_GRAPHICS_BIT = 0x00000001,

    /// <summary>
    /// Specifies that queues in this queue family support compute operations.
    /// </summary>
    VK_QUEUE_COMPUTE_BIT = 0x00000002,

    /// <summary>
    /// Specifies that queues in this queue family support transfer operations.
    /// </summary>
    VK_QUEUE_TRANSFER_BIT = 0x00000004,

    /// <summary>
    /// Specifies that queues in this queue family support sparse memory management operations (see Sparse Resources). If any of the sparse resource features are enabled, then at least one queue family must support this bit.
    /// </summary>
    VK_QUEUE_SPARSE_BINDING_BIT = 0x00000008,

    /// <summary>
    /// Specifies that queues in this queue family support <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#video-decode-operations">video decode operations</see>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_QUEUE_PROTECTED_BIT = 0x00000010,

    /// <summary>
    /// Specifies that queues in this queue family support <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#video-encode-operations">video encode operations</see>.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_QUEUE_VIDEO_DECODE_BIT_KHR = 0x00000020,

    #if VK_ENABLE_BETA_EXTENSIONS
    /// <summary>
    /// Specifies that queues in this queue family support optical flow operations.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_QUEUE_VIDEO_ENCODE_BIT_KHR = 0x00000040,

    #endif
    /// <summary>
    /// Specifies that queues in this queue family support the <see cref="VkDeviceQueueCreateFlagBits.VK_DEVICE_QUEUE_CREATE_PROTECTED_BIT"/> bit. (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-protected-memory">Protected Memory</see>). If the physical device supports the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-protectedMemory">protectedMemory</see> feature, at least one of its queue families must support this bit.
    /// </summary>
    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_QUEUE_OPTICAL_FLOW_BIT_NV = 0x00000100,
}
