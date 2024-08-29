using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public readonly struct RenderPipelineCreateInfo
{
    public readonly struct Attachment
    {
        public required VkImageCreateInfo       Image       { get; init; }
        public required VkAttachmentDescription Description { get; init; }
    }

    public readonly struct ColorAttachment
    {
        public required Attachment Color { get; init; }
        public Attachment? Resolve       { get; init; }
    }

    public readonly struct SubPass
    {
        public required VkPipelineBindPoint PipelineBindPoint      { get; init; }
        public ColorAttachment[]            ColorAttachments       { get; init; } = [];
        public Attachment?                  DepthStencilAttachment { get; init; }

        public SubPass() { }
    }

    public readonly SubPass[]             SubPasses           { get; init; } = [];
    public readonly VkSubpassDependency[] SubpassDependencies { get; init; } = [];

    public RenderPipelineCreateInfo() { }
}
