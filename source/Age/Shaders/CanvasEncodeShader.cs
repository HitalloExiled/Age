using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasEncodeShader(VkRenderPass renderPass, uint subpass, bool watch)
: CanvasShader($"{nameof(CanvasEncodeShader)}.slang", renderPass, subpass, StencilOp.None, watch)
{
    public override string              Name              { get; } = nameof(CanvasEncodeShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasEncodeShader(VkRenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }

    public CanvasEncodeShader(RenderTarget renderTarget, bool watch) : this(renderTarget.RenderPass, 0, watch)
    { }
}
