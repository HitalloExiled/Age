namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRenderPassBeginInfo.html">VkRenderPassBeginInfo</see>
/// </summary>
public unsafe struct VkRenderPassBeginInfo
{
    public VkStructureType sType;

    public void*         pNext;
    public VkRenderPass  renderPass;
    public VkFramebuffer framebuffer;
    public VkRect2D      renderArea;
    public uint          clearValueCount;
    public VkClearValue* pClearValues;

    public VkRenderPassBeginInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO;
}
