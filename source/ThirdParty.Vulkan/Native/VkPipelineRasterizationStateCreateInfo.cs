namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineRasterizationStateCreateInfo.html">VkPipelineRasterizationStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineRasterizationStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                   pNext;
    public VkPipelineRasterizationStateCreateFlags flags;
    public VkBool32                                depthClampEnable;
    public VkBool32                                rasterizerDiscardEnable;
    public VkPolygonMode                           polygonMode;
    public VkCullModeFlags                         cullMode;
    public VkFrontFace                             frontFace;
    public VkBool32                                depthBiasEnable;
    public float                                   depthBiasConstantFactor;
    public float                                   depthBiasClamp;
    public float                                   depthBiasSlopeFactor;
    public float                                   lineWidth;

    public VkPipelineRasterizationStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
}
