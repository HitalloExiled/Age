using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasIndexShader : CanvasShader
{
    public override string              Name              { get; } = nameof(CanvasIndexShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasIndexShader(RenderPass renderPass, uint subpass, bool watch) : base(renderPass, subpass, $"Canvas/{nameof(CanvasIndexShader)}.frag", StencilKind.Content, watch)
    { }

    public CanvasIndexShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }
}
