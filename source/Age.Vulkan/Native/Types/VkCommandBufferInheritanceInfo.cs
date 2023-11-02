using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying command buffer inheritance information</para>
/// <para>If the <see cref="VkCommandBuffer"/> will not be executed within a render pass instance, or if the render pass instance was begun with <see cref="Vk.CmdBeginRendering"/>, renderPass, subpass, and framebuffer are ignored.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkCommandBufferInheritanceInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A <see cref="VkRenderPass"/> object defining which render passes the <see cref="VkCommandBuffer"/> will be compatible with and can be executed within.
    /// </summary>
    public VkRenderPass renderPass;

    /// <summary>
    /// The index of the subpass within the render pass instance that the <see cref="VkCommandBuffer"/> will be executed within.
    /// </summary>
    public uint subpass;

    /// <summary>
    /// <para>Can refer to the <see cref="VkFramebuffer"/> object that the <see cref="VkCommandBuffer"/> will be rendering to if it is executed within a render pass instance. It can be VK_NULL_HANDLE if the framebuffer is not known.</para>
    /// <remarks>Note: Specifying the exact framebuffer that the secondary command buffer will be executed with may result in better performance at command buffer execution time.</remarks>
    /// </summary>
    public VkFramebuffer framebuffer;

    /// <summary>
    /// Specifies whether the command buffer can be executed while an occlusion query is active in the primary command buffer. If this is VK_TRUE, then this command buffer can be executed whether the primary command buffer has an occlusion query active or not. If this is VK_FALSE, then the primary command buffer must not have an occlusion query active.
    /// </summary>
    public VkBool32 occlusionQueryEnable;

    /// <summary>
    /// Specifies the query flags that can be used by an active occlusion query in the primary command buffer when this secondary command buffer is executed. If this value includes the <see cref="VkQueryControlFlagBits.VK_QUERY_CONTROL_PRECISE_BIT"/> bit, then the active query can return boolean results or actual sample counts. If this bit is not set, then the active query must not use the <see cref="VkQueryControlFlagBits.VK_QUERY_CONTROL_PRECISE_BIT"/> bit.
    /// </summary>
    public VkQueryControlFlags queryFlags;

    /// <summary>
    /// A bitmask of <see cref="VkQueryPipelineStatisticFlagBits"/> specifying the set of pipeline statistics that can be counted by an active query in the primary command buffer when this secondary command buffer is executed. If this value includes a given bit, then this command buffer can be executed whether the primary command buffer has a pipeline statistics query active that includes this bit or not. If this value excludes a given bit, then the active pipeline statistics query must not be from a query pool that counts that statistic.
    /// </summary>
    public VkQueryPipelineStatisticFlags pipelineStatistics;

    public VkCommandBufferInheritanceInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO;
}
