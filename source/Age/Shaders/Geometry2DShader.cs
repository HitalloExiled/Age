using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public abstract partial class Geometry2DShader(string file, VkRenderPass renderPass, uint subpass, StencilOp stencilOp, bool watch)
: Shader<Geometry2DShader.Vertex>(
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
{
    public override string              Name              { get; } = nameof(Geometry2DShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
