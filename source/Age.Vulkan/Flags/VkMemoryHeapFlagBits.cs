namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying attribute flags for a heap.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkMemoryHeapFlagBits
{
    /// <summary>
    /// Specifies that the heap corresponds to device-local memory. Device-local memory may have different performance characteristics than host-local memory, and may support different memory property flags.
    /// </summary>
    VK_MEMORY_HEAP_DEVICE_LOCAL_BIT = 0x00000001,

    /// <summary>
    /// Specifies that in a logical device representing more than one physical device, there is a per-physical device instance of the heap memory. By default, an allocation from such a heap will be replicated to each physical device’s instance of the heap.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_MEMORY_HEAP_MULTI_INSTANCE_BIT = 0x00000002,

    /// <inheritdoc cref="VK_MEMORY_HEAP_MULTI_INSTANCE_BIT" />
    /// <remarks>Provided by VK_KHR_device_group_creation</remarks>
    VK_MEMORY_HEAP_MULTI_INSTANCE_BIT_KHR = VK_MEMORY_HEAP_MULTI_INSTANCE_BIT,
}
