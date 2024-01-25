namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDepthStencilStateCreateInfo.html">VkPipelineDepthStencilStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineDepthStencilStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                  pNext;
    public VkPipelineDepthStencilStateCreateFlags flags;
    public VkBool32                               depthTestEnable;
    public VkBool32                               depthWriteEnable;
    public VkCompareOp                            depthCompareOp;
    public VkBool32                               depthBoundsTestEnable;
    public VkBool32                               stencilTestEnable;
    public VkStencilOpState                       front;
    public VkStencilOpState                       back;
    public float                                  minDepthBounds;
    public float                                  maxDepthBounds;

    public VkPipelineDepthStencilStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO;
}
