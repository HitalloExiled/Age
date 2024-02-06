namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetAllocateInfo.html">VkDescriptorSetAllocateInfo</see>
/// </summary>
public unsafe struct VkDescriptorSetAllocateInfo
{
    public readonly VkStructureType SType;

    public void*                            PNext;
    public VkHandle<VkDescriptorPool>       DescriptorPool;
    public uint                             DescriptorSetCount;
    public VkHandle<VkDescriptorSetLayout>* PSetLayouts;

    public VkDescriptorSetAllocateInfo() =>
        this.SType = VkStructureType.DescriptorSetAllocateInfo;
}
