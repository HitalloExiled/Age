using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDescription.html">VkSubpassDescription</see>
/// </summary>
public unsafe struct VkSubpassDescription
{
    public VkSubpassDescriptionFlags Flags;
    public VkPipelineBindPoint       PipelineBindPoint;
    public uint                      InputAttachmentCount;
    public VkAttachmentReference*    PInputAttachments;
    public uint                      ColorAttachmentCount;
    public VkAttachmentReference*    PColorAttachments;
    public VkAttachmentReference*    PResolveAttachments;
    public VkAttachmentReference*    PDepthStencilAttachment;
    public uint                      PreserveAttachmentCount;
    public uint*                     PPreserveAttachments;
}
