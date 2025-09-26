using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasIndexShader(VkRenderPass renderPass, uint subpass, bool watch)
: CanvasShader($"{nameof(CanvasIndexShader)}.slang", renderPass, subpass, StencilOp.None, watch)
{
    public override string              Name              { get; } = nameof(CanvasIndexShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasIndexShader(VkRenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }

    public CanvasIndexShader(RenderTarget renderTarget, bool watch) : this(renderTarget.RenderPass, 0, watch)
    { }
}
