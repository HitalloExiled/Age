using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineLayoutCreateInfo.html">VkPipelineLayoutCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineLayoutCreateInfo
{
    public readonly VkStructureType SType;

    public void*                            PNext;
    public VkPipelineLayoutCreateFlags      Flags;
    public uint                             SetLayoutCount;
    public VkHandle<VkDescriptorSetLayout>* PSetLayouts;
    public uint                             PushConstantRangeCount;
    public VkPushConstantRange*             PPushConstantRanges;

    public VkPipelineLayoutCreateInfo() =>
        this.SType = VkStructureType.PipelineLayoutCreateInfo;
}
