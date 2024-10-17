using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasStencilMaskShader(RenderPass renderPass, bool watch) : CanvasShader(renderPass, $"Canvas/{nameof(CanvasStencilMaskShader)}.frag", StencilKind.Mask, watch)
{
    public override string              Name              { get; } = nameof(CanvasStencilMaskShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
