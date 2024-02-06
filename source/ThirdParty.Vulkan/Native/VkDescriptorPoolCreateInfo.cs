using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolCreateInfo.html">VkDescriptorPoolCreateInfo</see>
/// </summary>
public unsafe struct VkDescriptorPoolCreateInfo
{
    public readonly VkStructureType SType;

    public void*                       PNext;
    public VkDescriptorPoolCreateFlags Flags;
    public uint                        MaxSets;
    public uint                        PoolSizeCount;
    public VkDescriptorPoolSize*       PPoolSizes;

    public VkDescriptorPoolCreateInfo() =>
        this.SType = VkStructureType.DescriptorPoolCreateInfo;
}
