using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutCreateInfo.html">VkDescriptorSetLayoutCreateInfo</see>
/// </summary>
public unsafe struct VkDescriptorSetLayoutCreateInfo
{
    public readonly VkStructureType SType;

    public void*                            PNext;
    public VkDescriptorSetLayoutCreateFlags Flags;
    public uint                             BindingCount;
    public VkDescriptorSetLayoutBinding*    PBindings;

    public VkDescriptorSetLayoutCreateInfo() =>
        this.SType = VkStructureType.DescriptorSetLayoutCreateInfo;
}
