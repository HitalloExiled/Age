using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader : Shader<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    protected CanvasShader(RenderPass renderPass, uint subpass, string fragmentShader, StencilKind stencil, bool watch)
    : base(
        renderPass,
        [$"Canvas/{nameof(CanvasShader)}.vert", fragmentShader],
        new()
        {
            FrontFace            = VkFrontFace.Clockwise,
            RasterizationSamples = ThirdParty.Vulkan.Flags.VkSampleCountFlags.N1,
            Stencil              = stencil,
            Subpass              = subpass,
            Watch                = watch,
        }
    )
    { }

    public CanvasShader(RenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }

    public CanvasShader(RenderPass renderPass, uint subpass, bool watch) : this(renderPass, subpass, $"Canvas/{nameof(CanvasShader)}.frag", StencilKind.Content, watch)
    { }
}
