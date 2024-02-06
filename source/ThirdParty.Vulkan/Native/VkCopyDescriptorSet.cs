namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCopyDescriptorSet.html">VkCopyDescriptorSet</see>
/// </summary>
public unsafe struct VkCopyDescriptorSet
{
    public readonly VkStructureType SType;

    public void*                     PNext;
    public VkHandle<VkDescriptorSet> SrcSet;
    public uint                      SrcBinding;
    public uint                      SrcArrayElement;
    public VkHandle<VkDescriptorSet> DstSet;
    public uint                      DstBinding;
    public uint                      DstArrayElement;
    public uint                      DescriptorCount;

    public VkCopyDescriptorSet() =>
        this.SType = VkStructureType.CopyDescriptorSet;
}
