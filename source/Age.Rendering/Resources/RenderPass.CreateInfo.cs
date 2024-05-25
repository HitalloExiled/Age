using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public partial record RenderPass
{
    public readonly ref partial struct CreateInfo
    {
        public readonly struct ColorAttachment
        {
            public VkImageLayout            Layout  { get; init; }
            public VkAttachmentDescription  Color   { get; init; }
            public VkAttachmentDescription? Resolve { get; init; }
        };

        public readonly struct SubPass
        {
            public required VkImage[]           Images                 { get; init; } = [];
            public required VkImageAspectFlags  ImageAspect            { get; init; }
            public required VkFormat            Format                 { get; init; }
            public required VkPipelineBindPoint PipelineBindPoint      { get; init; }
            public ColorAttachment[]            ColorAttachments       { get; init; } = [];
            public VkAttachmentReference?       DepthStencilAttachment { get; init; }

            public SubPass() { }
        }

        public required int          FrameBufferCount    { get; init; }
        public required VkExtent2D   Extent              { get; init; }
        public required SubPass[]    SubPasses           { get; init; } = [];
        public VkSubpassDependency[] SubpassDependencies { get; init; } = [];

        public CreateInfo() { }
    }
}
