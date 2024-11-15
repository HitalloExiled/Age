using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public sealed partial class CanvasStencilMaskShader(RenderPass renderPass, uint subpass, bool watch) : CanvasShader(renderPass, subpass, $"Canvas/{nameof(CanvasStencilMaskShader)}.frag", StencilKind.Mask, watch)
{
    public override string              Name              { get; } = nameof(CanvasStencilMaskShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasStencilMaskShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }
}
