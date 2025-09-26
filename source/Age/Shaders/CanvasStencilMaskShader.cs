using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasStencilMaskShader(VkRenderPass renderPass, StencilOp stencilOp, uint subpass, bool watch)
: CanvasShader($"{nameof(CanvasStencilMaskShader)}.slang", renderPass, subpass, stencilOp, watch)
{
    public override string              Name              { get; } = nameof(CanvasStencilMaskShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasStencilMaskShader(RenderPass renderPass, StencilOp stencilOp, bool watch) : this(renderPass, stencilOp, 0, watch)
    { }

    public CanvasStencilMaskShader(RenderTarget renderTarget, StencilOp stencilOp, bool watch) : this(renderTarget.RenderPass, stencilOp, 0, watch)
    { }
}
