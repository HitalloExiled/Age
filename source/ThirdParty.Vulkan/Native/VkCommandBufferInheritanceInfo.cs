namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferInheritanceInfo.html">VkCommandBufferInheritanceInfo</see>
/// </summary>
public unsafe struct VkCommandBufferInheritanceInfo
{
    public readonly VkStructureType sType;

    public void*                         pNext;
    public VkRenderPass                  renderPass;
    public uint                          subpass;
    public VkFramebuffer                 framebuffer;
    public VkBool32                      occlusionQueryEnable;
    public VkQueryControlFlags           queryFlags;
    public VkQueryPipelineStatisticFlags pipelineStatistics;

    public VkCommandBufferInheritanceInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO;
}
