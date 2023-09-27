using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying a copy descriptor set operation.</para>
/// <para>If the <see cref="VkDescriptorSetLayoutBinding"/> for dstBinding is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/> and srcBinding is not <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/>, the new active descriptor type becomes the descriptor type of srcBinding. If both <see cref="VkDescriptorSetLayoutBinding"/> for srcBinding and dstBinding are <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/>, the active descriptor type in each source descriptor is copied into the corresponding destination descriptor. The active descriptor type can be different for each source descriptor.</para>
/// <para>Note: The intention is that copies to and from mutable descriptors is a simple memcpy. Copies between non-mutable and mutable descriptors are expected to require one memcpy per descriptor to handle the difference in size, but this use case with more than one descriptorCount is considered rare.</para>
/// </summary>
public unsafe struct VkCopyDescriptorSet
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// srcSet, srcBinding, and srcArrayElement are the source set, binding, and array element, respectively. If the descriptor binding identified by srcSet and srcBinding has a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> then srcArrayElement specifies the starting byte offset within the binding to copy from.
    /// </summary>
    public VkDescriptorSet srcSet;

    /// <inheritdoc cref="srcSet" />
    public uint srcBinding;

    /// <inheritdoc cref="srcSet" />
    public uint srcArrayElement;

    /// <summary>
    /// dstSet, dstBinding, and dstArrayElement are the destination set, binding, and array element, respectively. If the descriptor binding identified by dstSet and dstBinding has a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> then dstArrayElement specifies the starting byte offset within the binding to copy to.
    /// </summary>
    public VkDescriptorSet dstSet;

    /// <inheritdoc cref="dstSet" />
    public uint dstBinding;

    /// <inheritdoc cref="dstSet" />
    public uint dstArrayElement;

    /// descriptorCount is the number of descriptors to copy from the source to destination. If descriptorCount is greater than the number of remaining array elements in the source or destination binding, those affect consecutive bindings in a manner similar to VkWriteDescriptorSet above. If the descriptor binding identified by srcSet and srcBinding has a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> then descriptorCount specifies the number of bytes to copy and the remaining array elements in the source or destination binding refer to the remaining number of bytes in those.
    public uint descriptorCount;

    public VkCopyDescriptorSet() =>
        sType = VkStructureType.VK_STRUCTURE_TYPE_COPY_DESCRIPTOR_SET;
}
