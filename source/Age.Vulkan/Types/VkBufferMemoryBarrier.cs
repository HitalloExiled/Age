using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying a buffer memory barrier.</para>
/// <para>The first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access to memory through the specified buffer range, via access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">source access mask</see> specified by srcAccessMask. If srcAccessMask includes <see cref="VK_ACCESS_HOST_WRITE_BIT"/>, a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-available-and-visible">memory domain operation</see> is performed where available memory in the host domain is also made available to the device domain.</para>
/// <para>The second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access to memory through the specified buffer range, via access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">destination access mask</see> specified by dstAccessMask. If dstAccessMask includes <see cref="VK_ACCESS_HOST_WRITE_BIT"/> or <see cref="VK_ACCESS_HOST_READ_BIT"/>, a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-available-and-visible">memory domain operation</see> is performed where available memory in the device domain is also made available to the host domain.</para>
/// <remarks>Note: When <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> is used, available memory in host domain is automatically made visible to host domain, and any host write is automatically made available to host domain.</remarks>
/// <para>If srcQueueFamilyIndex is not equal to dstQueueFamilyIndex, and srcQueueFamilyIndex is equal to the current queue family, then the memory barrier defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-queue-transfers-release">queue family release operation</see> for the specified buffer range, and the second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> includes no access, as if dstAccessMask was 0.</para>
/// <para>If dstQueueFamilyIndex is not equal to srcQueueFamilyIndex, and dstQueueFamilyIndex is equal to the current queue family, then the memory barrier defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-queue-transfers-acquire">queue family acquire operation</see> for the specified buffer range, and the first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> includes no access, as if srcAccessMask was 0.</para>
/// </summary>
public unsafe struct VkBufferMemoryBarrier
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a source access mask.
    /// </summary>
    public VkAccessFlags srcAccessMask;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a destination access mask.
    /// </summary>
    public VkAccessFlags dstAccessMask;

    /// <summary>
    /// The source queue family for a queue family ownership transfer.
    /// </summary>
    public uint srcQueueFamilyIndex;

    /// <summary>
    /// The destination queue family for a queue family ownership transfer.
    /// </summary>
    public uint dstQueueFamilyIndex;

    /// <summary>
    /// A handle to the buffer whose backing memory is affected by the barrier.
    /// </summary>
    public VkBuffer buffer;

    /// <summary>
    /// An offset in bytes into the backing memory for buffer; this is relative to the base offset as bound to the buffer (see <see cref="Vk.BindBufferMemory"/>).
    /// </summary>
    public VkDeviceSize offset;

    /// <summary>
    /// A size in bytes of the affected area of backing memory for buffer, or <see cref="Vk.VK_WHOLE_SIZE"/> to use the range from offset to the end of the buffer.
    /// </summary>
    public VkDeviceSize size;

    public VkBufferMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER;
}
