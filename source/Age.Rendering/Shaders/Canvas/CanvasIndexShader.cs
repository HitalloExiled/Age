using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasIndexShader(RenderPass renderPass, bool watch)
: Shader<CanvasShader.Vertex, CanvasShader.PushConstant>(
    renderPass,
    [$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasIndexShader)}.frag"],
    watch
)
{
    public override string              Name              { get; } = nameof(CanvasIndexShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
