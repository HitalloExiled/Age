using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class CanvasShader : Shader<CanvasShader.Vertex>
{
    public override string              Name               { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint          { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology  { get; } = VkPrimitiveTopology.TriangleList;

    protected CanvasShader(string file, RenderPass renderPass, uint subpass, StencilKind stencil, bool watch)
    : base(
        file,
        renderPass,
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

    public CanvasShader(RenderPass renderPass, bool watch)
    : this(renderPass, 0, watch)
    { }

    public CanvasShader(RenderPass renderPass, uint subpass, bool watch)
    : this($"{nameof(CanvasShader)}.slang", renderPass, subpass, StencilKind.Content, watch)
    { }
}
