using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying physical device memory properties.</para>
/// <para>The <see cref="VkPhysicalDeviceMemoryProperties"/> structure describes a number of memory heaps as well as a number of memory types that can be used to access memory allocated in those heaps. Each heap describes a memory resource of a particular size, and each memory type describes a set of memory properties (e.g. host cached vs. uncached) that can be used with a given memory heap. Allocations using a particular memory type will consume resources from the heap indicated by that memory type’s heap index. More than one memory type may share each heap, and the heaps and memory types provide a mechanism to advertise an accurate size of the physical memory resources while allowing the memory to be used with a variety of different properties.</para>
/// <para>The number of memory heaps is given by memoryHeapCount and is less than or equal to <see cref="Vk.VK_MAX_MEMORY_HEAPS"/>. Each heap is described by an element of the memoryHeaps array as a <see cref="VkMemoryHeap"/> structure. The number of memory types available across all memory heaps is given by memoryTypeCount and is less than or equal to <see cref="Vk.VK_MAX_MEMORY_TYPES"/>. Each memory type is described by an element of the memoryTypes array as a <see cref="VkMemoryType"/> structure.</para>
/// <para>At least one heap must include <see cref="VkMemoryHeapFlagBits.VK_MEMORY_HEAP_DEVICE_LOCAL_BIT"/> in <see cref="VkMemoryHeap.flags"/>. If there are multiple heaps that all have similar performance characteristics, they may all include <see cref="VkMemoryHeapFlagBits.VK_MEMORY_HEAP_DEVICE_LOCAL_BIT"/>. In a unified memory architecture (UMA) system there is often only a single memory heap which is considered to be equally “local” to the host and to the device, and such an implementation must advertise the heap as device-local.</para>
/// <para>Each memory type returned by <see cref="Vk.GetPhysicalDeviceMemoryProperties"/> must have its propertyFlags set to one of the following values:</para>
/// <list type="bullet">
/// <item>0</item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_PROTECTED_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_PROTECTED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_CACHED_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/></item>
/// <item><see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_RDMA_CAPABLE_BIT_NV"/></item>
/// </list>
/// <para>There must be at least one memory type with both the <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> and <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> bits set in its propertyFlags. There must be at least one memory type with the <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> bit set in its propertyFlags. If the deviceCoherentMemory feature is enabled, there must be at least one memory type with the <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> bit set in its propertyFlags.</para>
/// <para>For each pair of elements X and Y returned in memoryTypes, X must be placed at a lower index position than Y if:</para>
/// <list type="bullet">
/// <item>the set of bit flags returned in the propertyFlags member of X is a strict subset of the set of bit flags returned in the propertyFlags member of Y; or</item>
/// <item>the propertyFlags members of X and Y are equal, and X belongs to a memory heap with greater performance (as determined in an implementation-specific manner) ; or</item>
/// <item>the propertyFlags members of Y includes <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_COHERENT_BIT_AMD"/> or <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_UNCACHED_BIT_AMD"/> and X does not</item>
/// </list>
/// <remarks>Note: There is no ordering requirement between X and Y elements for the case their propertyFlags members are not in a subset relation. That potentially allows more than one possible way to order the same set of memory types. Notice that the list of all allowed memory property flag combinations is written in a valid order. But if instead <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT"/> was before <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT"/> | <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/>, the list would still be in a valid order.</remarks>
/// There may be a performance penalty for using device coherent or uncached device memory types, and using these accidentally is undesirable. In order to avoid this, memory types with these properties always appear at the end of the list; but are subject to the same rules otherwise.
/// </summary>
public unsafe struct VkPhysicalDeviceMemoryProperties
{
    /// <summary>
    /// The number of valid elements in the memoryTypes array.
    /// </summary>
    public uint memoryTypeCount;

    /// <summary>
    /// An array of <see cref="Vk.VK_MAX_MEMORY_TYPES"/> <see cref="VkMemoryType"/> structures describing the memory types that can be used to access memory allocated from the heaps specified by memoryHeaps.
    /// </summary>
    public fixed byte memoryTypes[8 * (int)Vk.VK_MAX_MEMORY_TYPES];

    /// <summary>
    /// The number of valid elements in the memoryHeaps array.
    /// </summary>
    public uint memoryHeapCount;

    /// <summary>
    /// An array of <see cref="Vk.VK_MAX_MEMORY_HEAPS"/> <see cref="VkMemoryHeap"/> structures describing the memory heaps from which memory can be allocated.
    /// </summary>
    public fixed byte memoryHeaps[16 * (int)Vk.VK_MAX_MEMORY_HEAPS];

    public VkMemoryType GetMemoryTypes(uint index)
    {
        fixed (byte* pMemoryTypes = this.memoryTypes)
        {
            return Unsafe.Read<VkMemoryType>(pMemoryTypes + sizeof(VkMemoryType) * index);
        }
    }

    public VkMemoryHeap GetMemoryHeaps(uint index)
    {
        fixed (byte* pMemoryHeaps = this.memoryHeaps)
        {
            return Unsafe.Read<VkMemoryHeap>(pMemoryHeaps + sizeof(VkMemoryHeap) * index);
        }
    }

    public void SetMemoryTypes(uint index, in VkMemoryType value)
    {
        fixed (byte* pMemoryTypes = this.memoryTypes)
        {
            Unsafe.Write(pMemoryTypes + sizeof(VkMemoryType) * index, value);
        }
    }

    public void SetMemoryHeaps(uint index, in VkMemoryHeap value)
    {
        fixed (byte* pMemoryHeaps = this.memoryHeaps)
        {
            Unsafe.Write(pMemoryHeaps + sizeof(VkMemoryHeap) * index, value);
        }
    }
}
