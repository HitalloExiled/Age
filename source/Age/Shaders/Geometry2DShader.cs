using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public abstract partial class Geometry2DShader(string file, VkRenderPass renderPass, ShaderOptions shaderOptions)
: Shader<Geometry2DShader.Vertex>(
    file,
    renderPass,
    shaderOptions with
    {
        FrontFace = VkFrontFace.Clockwise,
    }
)
{
    public override string              Name              { get; } = nameof(Geometry2DShader);
    public override VkPipelineBindPoint BindPoint         { get; } = VkPipelineBindPoint.Graphics;
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
