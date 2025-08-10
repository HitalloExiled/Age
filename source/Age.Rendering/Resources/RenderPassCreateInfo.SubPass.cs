using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public readonly partial struct RenderPassCreateInfo
{
    public readonly struct SubPass()
    {
        public required VkPipelineBindPoint PipelineBindPoint      { get; init; }
        public ColorAttachment[]            ColorAttachments       { get; init; } = [];
        public VkAttachmentDescription?     DepthStencilAttachment { get; init; }
    }
}
