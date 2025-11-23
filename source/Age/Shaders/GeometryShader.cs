using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed partial class GeometryShader(VkRenderPass renderPass, SampleCount rasterizationSamples, bool watch)
: Shader<GeometryShader.Vertex>(
    $"{nameof(GeometryShader)}.slang",
    renderPass,
    new ShaderOptions
    {
        Watch                = watch,
        FrontFace            = VkFrontFace.CounterClockwise,
        RasterizationSamples = rasterizationSamples
    }
)
{
    public override string              Name               { get; } = nameof(GeometryShader);
    public override VkPipelineBindPoint BindPoint          { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology  { get; } = VkPrimitiveTopology.TriangleList;
}
