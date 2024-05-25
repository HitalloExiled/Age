#define DUMP_IMAGES

using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Rendering.Services;

internal partial class RenderingService
{
    private struct RenderContext
    {
        public required RenderFlags     Flags;
        public required VkCommandBuffer CommandBuffer;
        public required IndexBuffer     IndexBuffer;
        public required VertexBuffer    VertexBuffer;
        public required RenderPass      RenderPass;
        public required Shader          Shader;

        public RenderContext[] Next = [];

        public RenderContext()
        { }
    }
}
