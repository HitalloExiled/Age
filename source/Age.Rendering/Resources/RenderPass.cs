using Age.Core;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public partial class RenderPass : Disposable
{
    public required VkRenderPass  Value     { get; init; }
    public required SubPass[]     SubPasses { get; init; }

    protected override void OnDispose() =>
        this.Value.Dispose();

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


