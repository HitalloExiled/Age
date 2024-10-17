using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader : Shader<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    protected CanvasShader(RenderPass renderPass, string fragmentShader, StencilKind stencil, bool watch)
    : base(
        renderPass,
        [$"Canvas/{nameof(CanvasShader)}.vert", fragmentShader],
        new()
        {
            RasterizationSamples = ThirdParty.Vulkan.Flags.VkSampleCountFlags.N1,
            FrontFace            = VkFrontFace.Clockwise,
            Watch                = watch,
            Stencil              = stencil,
        }
    )
    { }

    public CanvasShader(RenderPass renderPass, bool watch) : this(renderPass, $"Canvas/{nameof(CanvasShader)}.frag", default, watch)
    { }
}
