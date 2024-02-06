using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFramebufferCreateInfo.html">VkFramebufferCreateInfo</see>
/// </summary>
public unsafe struct VkFramebufferCreateInfo
{
    public readonly VkStructureType SType;

    public void*                    PNext;
    public VkFramebufferCreateFlags Flags;
    public VkHandle<VkRenderPass>   RenderPass;
    public uint                     AttachmentCount;
    public VkHandle<VkImageView>*   PAttachments;
    public uint                     Width;
    public uint                     Height;
    public uint                     Layers;

    public VkFramebufferCreateInfo() =>
        this.SType = VkStructureType.FramebufferCreateInfo;
}
