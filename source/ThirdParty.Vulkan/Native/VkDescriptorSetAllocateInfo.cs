namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetAllocateInfo.html">VkDescriptorSetAllocateInfo</see>
/// </summary>
public unsafe struct VkDescriptorSetAllocateInfo
{
    public readonly VkStructureType sType;

    public void*                  pNext;
    public VkDescriptorPool       descriptorPool;
    public uint                   descriptorSetCount;
    public VkDescriptorSetLayout* pSetLayouts;

    public VkDescriptorSetAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO;
}
