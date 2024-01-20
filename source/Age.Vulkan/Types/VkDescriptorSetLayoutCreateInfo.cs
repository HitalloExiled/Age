using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying parameters of a newly created descriptor set layout.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkDescriptorSetLayoutCreateInfo
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
    /// A bitmask of <see cref="VkDescriptorSetLayoutCreateFlagBits"/> specifying options for descriptor set layout creation.
    /// </summary>
    public VkDescriptorSetLayoutCreateFlags flags;

    /// <summary>
    /// The number of elements in pBindings.
    /// </summary>
    public uint bindingCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkDescriptorSetLayoutBinding"/> structures.
    /// </summary>
    public VkDescriptorSetLayoutBinding* pBindings;

    public VkDescriptorSetLayoutCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO;
}
