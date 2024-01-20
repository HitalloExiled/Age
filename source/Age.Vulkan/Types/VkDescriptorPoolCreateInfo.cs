using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created descriptor pool.</para>
/// <para>If multiple <see cref="VkDescriptorPoolSize"/> structures containing the same descriptor type appear in the pPoolSizes array then the pool will be created with enough storage for the total number of descriptors of each type.</para>
/// <para>Fragmentation of a descriptor pool is possible and may lead to descriptor set allocation failures. A failure due to fragmentation is defined as failing a descriptor set allocation despite the sum of all outstanding descriptor set allocations from the pool plus the requested allocation requiring no more than the total number of descriptors requested at pool creation. Implementations provide certain guarantees of when fragmentation must not cause allocation failure, as described below.</para>
/// <para>If a descriptor pool has not had any descriptor sets freed since it was created or most recently reset then fragmentation must not cause an allocation failure (note that this is always the case for a pool created without the <see cref="VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT"/> bit set). Additionally, if all sets allocated from the pool since it was created or most recently reset use the same number of descriptors (of each type) and the requested allocation also uses that same number of descriptors (of each type), then fragmentation must not cause an allocation failure.</para>
/// <para>If an allocation failure occurs due to fragmentation, an application can create an additional descriptor pool to perform further descriptor set allocations.</para>
/// <para>If flags has the <see cref="VkDescriptorPoolCreateFlagBits.VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT"/> bit set, descriptor pool creation may fail with the error <see cref="VkResult.VK_ERROR_FRAGMENTATION"/> if the total number of descriptors across all pools (including this one) created with this bit set exceeds maxUpdateAfterBindDescriptorsInAllPools, or if fragmentation of the underlying hardware resources occurs.</para>
/// <para>If a pPoolSizes[i].type is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/>, a <see cref="VkMutableDescriptorTypeCreateInfoEXT"/> struct in the pNext chain can be used to specify which mutable descriptor types can be allocated from the pool. If included in the pNext chain, <see cref="VkMutableDescriptorTypeCreateInfoEXT.pMutableDescriptorTypeLists[i]"/> specifies which kind of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/> descriptors can be allocated from this pool entry. If <see cref="VkMutableDescriptorTypeCreateInfoEXT"/> does not exist in the pNext chain, or <see cref="VkMutableDescriptorTypeCreateInfoEXT.pMutableDescriptorTypeLists[i]"/> is out of range, the descriptor pool allocates enough memory to be able to allocate a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/> descriptor with any supported <see cref="VkDescriptorType"/> as a mutable descriptor. A mutable descriptor can be allocated from a pool entry if the type list in <see cref="VkDescriptorSetLayoutCreateInfo"/> is a subset of the type list declared in the descriptor pool, or if the pool entry is created without a descriptor type list. Multiple pPoolSizes entries with <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/> can be declared. When multiple such pool entries are present in pPoolSizes, they specify sets of supported descriptor types which either fully overlap, partially overlap, or are disjoint. Two sets fully overlap if the sets of supported descriptor types are equal. If the sets are not disjoint they partially overlap. A pool entry without a <see cref="VkMutableDescriptorTypeListEXT"/> assigned to it is considered to partially overlap any other pool entry which has a <see cref="VkMutableDescriptorTypeListEXT"/> assigned to it. The application must ensure that partial overlap does not exist in pPoolSizes.</para>
/// <remarks>Note: The requirement of no partial overlap is intended to resolve ambiguity for validation as there is no confusion which pPoolSizes entries will be allocated from. An implementation is not expected to depend on this requirement.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkDescriptorPoolCreateInfo
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
    /// A bitmask of <see cref="VkDescriptorPoolCreateFlagBits"/> specifying certain supported operations on the pool.
    /// </summary>
    public VkDescriptorPoolCreateFlags flags;

    /// <summary>
    /// The maximum number of descriptor sets that can be allocated from the pool.
    /// </summary>
    public uint maxSets;

    /// <summary>
    /// The number of elements in pPoolSizes.
    /// </summary>
    public uint poolSizeCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkDescriptorPoolSize"/> structures, each containing a descriptor type and number of descriptors of that type to be allocated in the pool.
    /// </summary>
    public VkDescriptorPoolSize* pPoolSizes;

    public VkDescriptorPoolCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO;
}
