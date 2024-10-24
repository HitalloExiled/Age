using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public class CanvasWireframeShader : CanvasShader
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    public CanvasWireframeShader(RenderPass renderPass, uint subpass, bool watch) : base(renderPass, subpass, $"Canvas/{nameof(CanvasWireframeShader)}.frag", StencilKind.Content, watch)
    { }

    public CanvasWireframeShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }
}
