using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public partial class RenderPass
{
    public class SubPass
    {
        public struct Attachment
        {
            public required VkFormat           Format;
            public required VkSampleCountFlags Samples;
        }

        public struct ColorAttachment
        {
            public Attachment  Color;
            public Attachment? Resolve;
        }

        public required VkPipelineBindPoint PipelineBindPoint      { get; init; }
        public required ColorAttachment[]   ColorAttachments       { get; init; }
        public required Attachment?         DepthStencilAttachment { get; init; }
    }
}
