namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineViewportStateCreateInfo.html">VkPipelineViewportStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineViewportStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                              pNext;
    public VkPipelineViewportStateCreateFlags flags;
    public uint                               viewportCount;
    public VkViewport*                        pViewports;
    public uint                               scissorCount;
    public VkRect2D*                          pScissors;

    public VkPipelineViewportStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
}
