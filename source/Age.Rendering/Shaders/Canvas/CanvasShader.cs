using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader : Shader<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    protected CanvasShader(RenderPass renderPass, string frag, StencilKind stencil, bool watch)
    : base(
        renderPass,
        [$"Canvas/{nameof(CanvasShader)}.vert", frag],
        new()
        {
            RasterizationSamples = ThirdParty.Vulkan.Flags.VkSampleCountFlags.N1,
            FrontFace            = VkFrontFace.Clockwise,
            Watch                = watch,
            Stencil              = stencil,
        }
    )
    { }

    public CanvasShader(RenderPass renderPass, bool watch)
    : base(
        renderPass,
        [$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasShader)}.frag"],
        new()
        {
            RasterizationSamples = ThirdParty.Vulkan.Flags.VkSampleCountFlags.N1,
            FrontFace            = VkFrontFace.Clockwise,
            Watch                = watch,
            Stencil              = default,
        }
    )
    { }
}
