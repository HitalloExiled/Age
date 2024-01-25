namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutCreateInfo.html">VkDescriptorSetLayoutCreateInfo</see>
/// </summary>
public unsafe struct VkDescriptorSetLayoutCreateInfo
{
    public readonly VkStructureType sType;

    public void*                            pNext;
    public VkDescriptorSetLayoutCreateFlags flags;
    public uint                             bindingCount;
    public VkDescriptorSetLayoutBinding*    pBindings;

    public VkDescriptorSetLayoutCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO;
}
