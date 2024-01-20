using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying the parameters of a descriptor set write operation.</para>
/// <para>Only one of pImageInfo, pBufferInfo, or pTexelBufferView members is used according to the descriptor type specified in the descriptorType member of the containing <see cref="VkWriteDescriptorSet"/> structure, or none of them in case descriptorType is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/>, in which case the source data for the descriptor writes is taken from the <see cref="VkWriteDescriptorSetInlineUniformBlock"/> structure included in the pNext chain of <see cref="VkWriteDescriptorSet"/>, or if descriptorType is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_ACCELERATION_STRUCTURE_KHR"/>, in which case the source data for the descriptor writes is taken from the <see cref="VkWriteDescriptorSetAccelerationStructureKHR"/> structure in the pNext chain of <see cref="VkWriteDescriptorSet"/>, or if descriptorType is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_ACCELERATION_STRUCTURE_NV"/>, in which case the source data for the descriptor writes is taken from the <see cref="VkWriteDescriptorSetAccelerationStructureNV"/> structure in the pNext chain of <see cref="VkWriteDescriptorSet"/>, as specified below.</para>
/// <para>If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-nullDescriptor">nullDescriptor</see> feature is enabled, the buffer, acceleration structure, imageView, or bufferView can be VK_NULL_HANDLE. Loads from a null descriptor return zero values and stores and atomics to a null descriptor are discarded. A null acceleration structure descriptor results in the miss shader being invoked.</para>
/// <para>If the destination descriptor is a mutable descriptor, the active descriptor type for the destination descriptor becomes descriptorType.</para>
/// <para>If the dstBinding has fewer than descriptorCount array elements remaining starting from dstArrayElement, then the remainder will be used to update the subsequent binding - dstBinding+1 starting at array element zero. If a binding has a descriptorCount of zero, it is skipped. This behavior applies recursively, with the update affecting consecutive bindings as needed to update all descriptorCount descriptors. Consecutive bindings must have identical <see cref="VkDescriptorType"/>, <see cref="VkShaderStageFlags"/>, <see cref="VkDescriptorBindingFlagBits"/>, and immutable samplers references. In addition, if the <see cref="VkDescriptorType"/> is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/>, the supported descriptor types in <see cref="VkMutableDescriptorTypeCreateInfoEXT"/> must be equally defined.</para>
/// <remarks>Note: The same behavior applies to bindings with a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> where descriptorCount specifies the number of bytes to update while dstArrayElement specifies the starting byte offset, thus in this case if the dstBinding has a smaller byte size than the sum of dstArrayElement and descriptorCount, then the remainder will be used to update the subsequent binding - dstBinding+1 starting at offset zero. This falls out as a special case of the above rule.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkWriteDescriptorSet
{
    /// sType is a <see cref="VkStructureType"/> value identifying this structure.
    public readonly VkStructureType sType;

    /// pNext is NULL or a pointer to a structure extending this structure.
    public void* pNext;

    /// <summary>
    /// The destination descriptor set to update.
    /// </summary>
    public VkDescriptorSet dstSet;

    /// <summary>
    /// The descriptor binding within that set.
    /// </summary>
    public uint dstBinding;

    /// <summary>
    /// The starting element in that array. If the descriptor binding identified by dstSet and dstBinding has a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> then dstArrayElement specifies the starting byte offset within the binding.
    /// </summary>
    public uint dstArrayElement;

    /// <summary>
    /// <para>The number of descriptors to update. If the descriptor binding identified by dstSet and dstBinding has a descriptor type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/>, then descriptorCount specifies the number of bytes to update. Otherwise, descriptorCount is one of</para>
    /// <list type="bullet">
    /// <item>the number of elements in pImageInfo</item>
    /// <item>the number of elements in pBufferInfo</item>
    /// <item>the number of elements in pTexelBufferView</item>
    /// <item>a value matching the dataSize member of a <see cref="VkWriteDescriptorSetInlineUniformBlock"/> structure in the pNext chain</item>
    /// <item>a value matching the accelerationStructureCount of a <see cref="VkWriteDescriptorSetAccelerationStructureKHR"/> structure in the pNext chain</item>
    /// </list>
    /// </summary>
    public uint descriptorCount;

    /// <summary>
    /// A <see cref="VkDescriptorType"/> specifying the type of each descriptor in pImageInfo, pBufferInfo, or pTexelBufferView, as described below. If <see cref="VkDescriptorSetLayoutBinding"/> for dstSet at dstBinding is not equal to <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_MUTABLE_EXT"/>, descriptorType must be the same type as the descriptorType specified in <see cref="VkDescriptorSetLayoutBinding"/> for dstSet at dstBinding. The type of the descriptor also controls which array the descriptors are taken from.
    /// </summary>
    public VkDescriptorType descriptorType;

    /// <summary>
    /// A pointer to an array of <see cref="VkDescriptorImageInfo"/> structures or is ignored, as described below.
    /// </summary>
    public VkDescriptorImageInfo* pImageInfo;

    /// <summary>
    /// A pointer to an array of <see cref="VkDescriptorBufferInfo"/> structures or is ignored, as described below.
    /// </summary>
    public VkDescriptorBufferInfo* pBufferInfo;

    /// <summary>
    /// A pointer to an array of <see cref="VkBufferView"/> handles as described in the Buffer Views section or is ignored, as described below.
    /// </summary>
    public VkBufferView* pTexelBufferView;

    public VkWriteDescriptorSet() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
}
