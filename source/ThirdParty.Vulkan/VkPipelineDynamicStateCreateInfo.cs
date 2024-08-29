using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDynamicStateCreateInfo.html">VkPipelineDynamicStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineDynamicStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                             PNext;
    public VkPipelineDynamicStateCreateFlags Flags;
    public uint                              DynamicStateCount;
    public VkDynamicState*                   PDynamicStates;

    public VkPipelineDynamicStateCreateInfo() =>
        this.sType = VkStructureType.PipelineDynamicStateCreateInfo;
}
