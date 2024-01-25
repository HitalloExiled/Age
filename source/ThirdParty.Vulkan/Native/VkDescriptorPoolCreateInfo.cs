namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolCreateInfo.html">VkDescriptorPoolCreateInfo</see>
/// </summary>
public unsafe struct VkDescriptorPoolCreateInfo
{
    public readonly VkStructureType sType;

    public void*                       pNext;
    public VkDescriptorPoolCreateFlags flags;
    public uint                        maxSets;
    public uint                        poolSizeCount;
    public VkDescriptorPoolSize*       pPoolSizes;

    public VkDescriptorPoolCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO;
}
