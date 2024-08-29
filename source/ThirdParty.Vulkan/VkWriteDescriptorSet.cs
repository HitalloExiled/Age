using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkWriteDescriptorSet.html">VkWriteDescriptorSet</see>
/// </summary>
public unsafe struct VkWriteDescriptorSet
{
    public readonly VkStructureType SType;

    public void*                     PNext;
    public VkHandle<VkDescriptorSet> DstSet;
    public uint                      DstBinding;
    public uint                      DstArrayElement;
    public uint                      DescriptorCount;
    public VkDescriptorType          DescriptorType;
    public VkDescriptorImageInfo*    PImageInfo;
    public VkDescriptorBufferInfo*   PBufferInfo;
    public VkHandle<VkBufferView>*   PTexelBufferView;

    public VkWriteDescriptorSet() =>
        this.SType = VkStructureType.WriteDescriptorSet;
}
