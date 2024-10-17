using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public class CanvasWireframeShader(RenderPass renderPass, bool watch)
: Shader<CanvasShader.Vertex, CanvasShader.PushConstant>(
    renderPass,
    [$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasWireframeShader)}.frag"],
    watch
)
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;
}
