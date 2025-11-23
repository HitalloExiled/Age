using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class CanvasShader : Shader<CanvasShader.Vertex>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    protected CanvasShader(string file, VkRenderPass renderPass, uint subpass, StencilOp stencilOp, bool watch)
    : base(
        file,
        renderPass,
        new()
        {
            FrontFace            = VkFrontFace.Clockwise,
            RasterizationSamples = SampleCount.N1,
            StencilOp            = stencilOp,
            Subpass              = subpass,
            Watch                = watch,
        }
    )
    { }

    public CanvasShader(VkRenderPass renderPass, bool watch)
    : this(renderPass, 0, watch)
    { }

    public CanvasShader(RenderTarget renderTarget, bool watch)
    : this(renderTarget.RenderPass, 0, watch)
    { }

    public CanvasShader(VkRenderPass renderPass, uint subpass, bool watch)
    : this($"{nameof(CanvasShader)}.slang", renderPass, subpass, StencilOp.None, watch)
    { }
}
