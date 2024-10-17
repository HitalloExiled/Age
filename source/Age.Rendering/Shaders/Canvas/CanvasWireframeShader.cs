using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public class CanvasWireframeShader(RenderPass renderPass, bool watch) : CanvasShader(renderPass, $"Canvas/{nameof(CanvasWireframeShader)}.frag", StencilKind.Content, watch)
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;
}
