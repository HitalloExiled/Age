using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed partial class Geometry3DShader(VkRenderPass renderPass, SampleCount rasterizationSamples, bool watch)
: Shader<Geometry3DShader.Vertex>(
    $"{nameof(Geometry3DShader)}.slang",
    renderPass,
    new ShaderOptions
    {
        Watch                = watch,
        FrontFace            = VkFrontFace.CounterClockwise,
        RasterizationSamples = rasterizationSamples
    }
)
{
    public override string              Name               { get; } = nameof(Geometry3DShader);
    public override VkPipelineBindPoint BindPoint          { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology  { get; } = VkPrimitiveTopology.TriangleList;
}
