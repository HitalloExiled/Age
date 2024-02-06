namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineVertexInputStateCreateInfo.html">VkPipelineVertexInputStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineVertexInputStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                 PNext;
    public VkPipelineVertexInputStateCreateFlags Flags;
    public uint                                  VertexBindingDescriptionCount;
    public VkVertexInputBindingDescription*      PVertexBindingDescriptions;
    public uint                                  VertexAttributeDescriptionCount;
    public VkVertexInputAttributeDescription*    PVertexAttributeDescriptions;

    public VkPipelineVertexInputStateCreateInfo() =>
        this.SType = VkStructureType.PipelineVertexInputStateCreateInfo;
}
