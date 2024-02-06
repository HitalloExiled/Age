namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRenderPassBeginInfo.html">VkRenderPassBeginInfo</see>
/// </summary>
public unsafe struct VkRenderPassBeginInfo
{
    public VkStructureType SType;

    public void*                   PNext;
    public VkHandle<VkRenderPass>  RenderPass;
    public VkHandle<VkFramebuffer> Framebuffer;
    public VkRect2D                RenderArea;
    public uint                    ClearValueCount;
    public VkClearValue*           PClearValues;

    public VkRenderPassBeginInfo() =>
        this.SType = VkStructureType.RenderPassBeginInfo;
}
