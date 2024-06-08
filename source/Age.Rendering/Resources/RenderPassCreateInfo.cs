using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public readonly struct RenderPassCreateInfo
{
    public readonly struct ColorAttachment
    {
        public VkAttachmentDescription  Color   { get; init; }
        public VkAttachmentDescription? Resolve { get; init; }
    };

    public readonly struct SubPass
    {
        public required VkPipelineBindPoint PipelineBindPoint    { get; init; }
        public ColorAttachment[]          ColorAttachments       { get; init; } = [];
        public VkAttachmentDescription?  DepthStencilAttachment  { get; init; }

        public SubPass() { }
    }

    public readonly SubPass[]             SubPasses           { get; init; } = [];
    public readonly VkSubpassDependency[] SubpassDependencies { get; init; } = [];

    public RenderPassCreateInfo() { }
}
