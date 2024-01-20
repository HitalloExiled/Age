namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying additional parameters of a buffer.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkBufferCreateFlagBits
{
    /// <summary>
    /// Specifies that the buffer will be backed using sparse memory binding.
    /// </summary>
    VK_BUFFER_CREATE_SPARSE_BINDING_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the buffer can be partially backed using sparse memory binding. Buffers created with this flag must also be created with the <see cref="VK_BUFFER_CREATE_SPARSE_BINDING_BIT"/> flag.
    /// </summary>
    VK_BUFFER_CREATE_SPARSE_RESIDENCY_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the buffer will be backed using sparse memory binding with memory ranges that might also simultaneously be backing another buffer (or another portion of the same buffer). Buffers created with this flag must also be created with the <see cref="VK_BUFFER_CREATE_SPARSE_BINDING_BIT"/> flag.
    /// </summary>
    VK_BUFFER_CREATE_SPARSE_ALIASED_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the buffer is a protected buffer.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_BUFFER_CREATE_PROTECTED_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the buffer’s address can be saved and reused on a subsequent run (e.g. for trace capture and replay), see <see cref="VkBufferOpaqueCaptureAddressCreateInfo"/> for more detail.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT = 0x00000010,

    /// <summary>
    /// Specifies that the buffer can be used with descriptor buffers when capturing and replaying (e.g. for trace capture and replay), see <see cref="VkOpaqueCaptureDescriptorDataCreateInfoEXT"/> for more detail.
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_BUFFER_CREATE_DESCRIPTOR_BUFFER_CAPTURE_REPLAY_BIT_EXT = 0x00000020,

    /// <inheritdoc cref="VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT" />
    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT_EXT = VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT,

    /// <inheritdoc cref="VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT" />
    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT_KHR = VK_BUFFER_CREATE_DEVICE_ADDRESS_CAPTURE_REPLAY_BIT,
}
