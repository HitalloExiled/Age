using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public partial class GeometryShader(RenderPass renderPass, VkSampleCountFlags rasterizationSamples, bool watch)
: Shader<GeometryShader.Vertex, GeometryShader.PushConstant>(
    renderPass,
    [$"{nameof(GeometryShader)}.vert", $"{nameof(GeometryShader)}.frag"],
    new ShaderOptions
    {
        Watch                = watch,
        FrontFace            = VkFrontFace.CounterClockwise,
        RasterizationSamples = rasterizationSamples
    }
)
{
    public override string              Name              { get; } = nameof(GeometryShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
