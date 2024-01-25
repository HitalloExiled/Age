namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineVertexInputStateCreateInfo.html">VkPipelineVertexInputStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineVertexInputStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                 pNext;
    public VkPipelineVertexInputStateCreateFlags flags;
    public uint                                  vertexBindingDescriptionCount;
    public VkVertexInputBindingDescription*      pVertexBindingDescriptions;
    public uint                                  vertexAttributeDescriptionCount;
    public VkVertexInputAttributeDescription*    pVertexAttributeDescriptions;

    public VkPipelineVertexInputStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO;
}
