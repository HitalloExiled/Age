using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed partial class CanvasIndexShader(RenderPass renderPass, uint subpass, bool watch)
: CanvasShader($"{nameof(CanvasIndexShader)}.slang", renderPass, subpass, StencilKind.Content, watch)
{
    public override string              Name              { get; } = nameof(CanvasIndexShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasIndexShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }
}
