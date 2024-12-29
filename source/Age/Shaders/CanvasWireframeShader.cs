using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasWireframeShader(RenderPass renderPass, uint subpass, bool watch)
: CanvasShader($"{nameof(CanvasWireframeShader)}.slang", renderPass, subpass, StencilKind.Content, watch)
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    public CanvasWireframeShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }
}
