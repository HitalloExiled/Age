using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed partial class Geometry3DShader(VkRenderPass renderPass, SampleCount rasterizationSamples)
: Shader<Geometry3DShader.Vertex>(
    $"{nameof(Geometry3DShader)}.slang",
    renderPass
)
{
    public override VkPipelineBindPoint BindPoint          => VkPipelineBindPoint.Graphics;
    public override VkFrontFace         FrontFace          => VkFrontFace.CounterClockwise;
    public override string              Name               => nameof(Geometry3DShader);
    public override VkPrimitiveTopology PrimitiveTopology  => VkPrimitiveTopology.TriangleList;
}
