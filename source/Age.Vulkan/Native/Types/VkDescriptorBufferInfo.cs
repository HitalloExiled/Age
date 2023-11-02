namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying descriptor buffer information.</para>
/// <para>For <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/> and <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/> descriptor types, offset is the base offset from which the dynamic offset is applied and range is the static size used for all dynamic offsets.</para>
/// <para>When range is <see cref="Vk.VK_WHOLE_SIZE"/> the effective range is calculated at vkUpdateDescriptorSets is by taking the size of buffer minus the offset.</para>
/// </summary>
public struct VkDescriptorBufferInfo
{
    /// <summary>
    /// Is VK_NULL_HANDLE or the buffer resource.
    /// </summary>
    public VkBuffer buffer;

    /// <summary>
    /// Is the offset in bytes from the start of buffer. Access to buffer memory via this descriptor uses addressing that is relative to this starting offset.
    /// </summary>
    public VkDeviceSize offset;

    /// <summary>
    /// <para>range Is the size in bytes that is used for this descriptor update, or <see cref="Vk.VK_WHOLE_SIZE"/> to use the range from offset to the end of the buffer.</para>
    /// <remarks>Note: When setting range to <see cref="Vk.VK_WHOLE_SIZE"/>, the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#buffer-info-effective-range">effective range</see> must not be larger than the maximum range for the descriptor type (<see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#limits-maxUniformBufferRange">maxUniformBufferRange</see> or <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#limits-maxStorageBufferRange">maxStorageBufferRange</see>). This means that VK_WHOLE_SIZE is not typically useful in the common case where uniform buffer descriptors are suballocated from a buffer that is much larger than maxUniformBufferRange.</remarks>
    /// </summary>
    public VkDeviceSize range;
}
