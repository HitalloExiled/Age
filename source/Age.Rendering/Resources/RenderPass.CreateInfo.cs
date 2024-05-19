using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

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

        public required VkImage[]  Images { get; init; }
        public required VkFormat   Format { get; init; }
        public required VkExtent2D Extent { get; init; }

        public Span<ColorAttachment>  ColorAttachments       { get; init; } = [];
        public VkAttachmentReference? DepthStencilAttachment { get; init; }

        public CreateInfo() { }
    }
}
