namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineMultisampleStateCreateInfo.html">VkPipelineMultisampleStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineMultisampleStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                 pNext;
    public VkPipelineMultisampleStateCreateFlags flags;
    public VkSampleCountFlagBits                 rasterizationSamples;
    public VkBool32                              sampleShadingEnable;
    public float                                 minSampleShading;
    public VkSampleMask*                         pSampleMask;
    public VkBool32                              alphaToCoverageEnable;
    public VkBool32                              alphaToOneEnable;

    public VkPipelineMultisampleStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
}
