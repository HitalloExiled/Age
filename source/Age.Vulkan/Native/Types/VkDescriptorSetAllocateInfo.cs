using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying the allocation parameters for descriptor sets.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkDescriptorSetAllocateInfo
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
    /// The pool which the sets will be allocated from.
    /// </summary>
    public VkDescriptorPool descriptorPool;

    /// <summary>
    /// Determines the number of descriptor sets to be allocated from the pool.
    /// </summary>
    public uint descriptorSetCount;

    /// <summary>
    /// A pointer to an array of descriptor set layouts, with each member specifying how the corresponding descriptor set is allocated.
    /// </summary>
    public VkDescriptorSetLayout* pSetLayouts;

    public VkDescriptorSetAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO;
}
