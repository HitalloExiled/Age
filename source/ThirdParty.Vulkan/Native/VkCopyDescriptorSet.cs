namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCopyDescriptorSet.html">VkCopyDescriptorSet</see>
/// </summary>
public unsafe struct VkCopyDescriptorSet
{
    public readonly VkStructureType sType;

    public void*           pNext;
    public VkDescriptorSet srcSet;
    public uint            srcBinding;
    public uint            srcArrayElement;
    public VkDescriptorSet dstSet;
    public uint            dstBinding;
    public uint            dstArrayElement;
    public uint            descriptorCount;

    public VkCopyDescriptorSet() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COPY_DESCRIPTOR_SET;
}
