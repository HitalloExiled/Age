using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader(RenderPass renderPass, bool watch)
: Shader<CanvasShader.Vertex, CanvasShader.PushConstant>(
    renderPass,
    [$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasShader)}.frag"],
    watch
)
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
