namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying certain supported operations on a descriptor pool.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkDescriptorPoolCreateFlagBits
{
    /// <summary>
    /// Specifies that descriptor sets can return their individual allocations to the pool, i.e. all of <see cref="Vk.AllocateDescriptorSets"/>, vkFreeDescriptorSets, and <see cref="Vk.ResetDescriptorPool"/> are allowed. Otherwise, descriptor sets allocated from the pool must not be individually freed back to the pool, i.e. only <see cref="Vk.AllocateDescriptorSets"/> and <see cref="Vk.ResetDescriptorPool"/> are allowed.
    /// </summary>
    VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT = 0x00000001,

    /// <summary>
    /// Specifies that descriptor sets allocated from this pool can include bindings with the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_UPDATE_AFTER_BIND_BIT"/> bit set. It is valid to allocate descriptor sets that have bindings that do not set the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_UPDATE_AFTER_BIND_BIT"/> bit from a pool that has <see cref="VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT"/> set.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT = 0x00000002,

    /// <summary>
    /// Specifies that this descriptor pool and the descriptor sets allocated from it reside entirely in host memory and cannot be bound. Similar to descriptor sets allocated without this flag, applications can copy-from and copy-to descriptors sets allocated from this descriptor pool. Descriptor sets allocated from this pool are partially exempt from the external synchronization requirement in <see cref="VkKhrDescriptorUpdateTemplate.UpdateDescriptorSetWithTemplate"/> and <see cref="Vk.UpdateDescriptorSets"/>. Descriptor sets and their descriptors can be updated concurrently in different threads, though the same descriptor must not be updated concurrently by two threads.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_POOL_CREATE_HOST_ONLY_BIT_EXT = 0x00000004,

    /// <summary>
    /// Specifies that the implementation should allow the application to allocate more than <see cref="VkDescriptorPoolCreateInfo.maxSets"/> descriptor set objects from the descriptor pool as available resources allow. The implementation may use the maxSets value to allocate the initial available sets, but using zero is permitted.
    /// </summary>
    /// <remarks>Provided by VK_NV_descriptor_pool_overallocation</remarks>
    VK_DESCRIPTOR_POOL_CREATE_ALLOW_OVERALLOCATION_SETS_BIT_NV = 0x00000008,

    /// <summary>
    /// Specifies that the implementation should allow the application to allocate more descriptors from the pool than was specified by the <see cref="VkDescriptorPoolSize.descriptorCount"/> for any descriptor type as specified by <see cref="VkDescriptorPoolCreateInfo.poolSizeCount"/> and <see cref="VkDescriptorPoolCreateInfo.pPoolSizes"/>, as available resources allow. The implementation may use the descriptorCount for each descriptor type to allocate the initial pool, but the application is allowed to set the poolSizeCount to zero, or any of the descriptorCount values in the pPoolSizes array to zero.
    /// </summary>
    /// <remarks>Provided by VK_NV_descriptor_pool_overallocation</remarks>
    VK_DESCRIPTOR_POOL_CREATE_ALLOW_OVERALLOCATION_POOLS_BIT_NV = 0x00000010,

    /// <inheritdoc cref="VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT" />
    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT_EXT = VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT,

    /// <inheritdoc cref="VK_DESCRIPTOR_POOL_CREATE_HOST_ONLY_BIT_EXT" />
    /// <remarks>Provided by VK_VALVE_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_POOL_CREATE_HOST_ONLY_BIT_VALVE = VK_DESCRIPTOR_POOL_CREATE_HOST_ONLY_BIT_EXT,
}
