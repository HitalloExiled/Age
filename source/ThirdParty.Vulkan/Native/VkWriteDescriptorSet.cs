namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkWriteDescriptorSet.html">VkWriteDescriptorSet</see>
/// </summary>
public unsafe struct VkWriteDescriptorSet
{
    public readonly VkStructureType sType;

    public void*                   pNext;
    public VkDescriptorSet         dstSet;
    public uint                    dstBinding;
    public uint                    dstArrayElement;
    public uint                    descriptorCount;
    public VkDescriptorType        descriptorType;
    public VkDescriptorImageInfo*  pImageInfo;
    public VkDescriptorBufferInfo* pBufferInfo;
    public VkBufferView*           pTexelBufferView;

    public VkWriteDescriptorSet() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
}
