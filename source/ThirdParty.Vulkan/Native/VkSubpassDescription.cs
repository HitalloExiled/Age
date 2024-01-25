namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDescription.html">VkSubpassDescription</see>
/// </summary>
public unsafe struct VkSubpassDescription
{
    public VkSubpassDescriptionFlags flags;
    public VkPipelineBindPoint       pipelineBindPoint;
    public uint                      inputAttachmentCount;
    public VkAttachmentReference*    pInputAttachments;
    public uint                      colorAttachmentCount;
    public VkAttachmentReference*    pColorAttachments;
    public VkAttachmentReference*    pResolveAttachments;
    public VkAttachmentReference*    pDepthStencilAttachment;
    public uint                      preserveAttachmentCount;
    public uint*                     pPreserveAttachments;
}
