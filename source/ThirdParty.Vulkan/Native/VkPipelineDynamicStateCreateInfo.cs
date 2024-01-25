namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDynamicStateCreateInfo.html">VkPipelineDynamicStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineDynamicStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                             pNext;
    public VkPipelineDynamicStateCreateFlags flags;
    public uint                              dynamicStateCount;
    public VkDynamicState*                   pDynamicStates;

    public VkPipelineDynamicStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
}
