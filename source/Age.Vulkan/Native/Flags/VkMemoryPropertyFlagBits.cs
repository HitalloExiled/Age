namespace Age.Vulkan.Native.Flags;

/// <summary>
/// <para>Bitmask specifying properties for a memory type.</para>
/// <para>For any memory allocated with both the <see cref="VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> and the <see cref="VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/>, host or device accesses also perform automatic memory domain transfer operations, such that writes are always automatically available and visible to both host and device memory domains.</para>
/// <remarks>Note: Device coherence is a useful property for certain debugging use cases (e.g. crash analysis, where performing separate coherence actions could mean values are not reported correctly). However, device coherent accesses may be slower than equivalent accesses without device coherence, particularly if they are also device uncached. For device uncached memory in particular, repeated accesses to the same or neighbouring memory locations over a short time period (e.g. within a frame) may be slower than it would be for the equivalent cached memory type. As such, it is generally inadvisable to use device coherent or device uncached memory except when really needed.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkMemoryPropertyFlagBits
{
    /// <summary>
    /// Specifies that memory allocated with this type is the most efficient for device access. This property will be set if and only if the memory type belongs to a heap with the <see cref="VK_MEMORY_HEAP_DEVICE_LOCAL_BIT"/> set.
    /// </summary>
    VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT = 0x00000001,

    /// <summary>
    /// Specifies that memory allocated with this type can be mapped for host access using vkMapMemory.
    /// </summary>
    VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the host cache management commands <see cref="Vk.FlushMappedMemoryRanges"/> and <see cref="Vk.InvalidateMappedMemoryRanges"/> are not needed to flush host writes to the device or make device writes visible to the host, respectively.
    /// </summary>
    VK_MEMORY_PROPERTY_HOST_COHERENT_BIT = 0x00000004,

    /// <summary>
    /// Specifies that memory allocated with this type is cached on the host. Host memory accesses to uncached memory are slower than to cached memory, however uncached memory is always host coherent.
    /// </summary>
    VK_MEMORY_PROPERTY_HOST_CACHED_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the memory type only allows device access to the memory. Memory types must not have both <see cref="VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT"/> and <see cref="VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> set. Additionally, the object’s backing memory may be provided by the implementation lazily as specified in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-device-lazy_allocation">Lazily Allocated Memory</see>.
    /// </summary>
    VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT = 0x00000010,

    /// <summary>
    /// Specifies that the memory type only allows device access to the memory, and allows protected queue operations to access the memory. Memory types must not have <see cref="VK_MEMORY_PROPERTY_PROTECTED_BIT"/> set and any of <see cref="VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> set, or <see cref="VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> set, or <see cref="VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> set.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_MEMORY_PROPERTY_PROTECTED_BIT = 0x00000020,


    /// <summary>
    /// Specifies that device accesses to allocations of this memory type are automatically made available and visible.
    /// </summary>
    /// <remarks>Provided by VK_AMD_device_coherent_memory</remarks>
    VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD = 0x00000040,


    /// <summary>
    /// Specifies that memory allocated with this type is not cached on the device. Uncached device memory is always device coherent.
    /// </summary>
    /// <remarks>Provided by VK_AMD_device_coherent_memory</remarks>
    VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD = 0x00000080,


    /// <summary>
    /// Specifies that external devices can access this memory directly.
    /// </summary>
    /// <remarks>Provided by VK_NV_external_memory_rdma</remarks>
    VK_MEMORY_PROPERTY_RDMA_CAPABLE_BIT_NV = 0x00000100,
}
