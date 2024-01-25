namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFramebufferCreateInfo.html">VkFramebufferCreateInfo</see>
/// </summary>
public unsafe struct VkFramebufferCreateInfo
{
    public readonly VkStructureType sType;

    public void*                    pNext;
    public VkFramebufferCreateFlags flags;
    public VkRenderPass             renderPass;
    public uint                     attachmentCount;
    public VkImageView*             pAttachments;
    public uint                     width;
    public uint                     height;
    public uint                     layers;

    public VkFramebufferCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
}
