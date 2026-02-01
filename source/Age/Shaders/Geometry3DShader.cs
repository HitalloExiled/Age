using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed partial class Geometry3DShader(VkRenderPass renderPass)
: Shader<Geometry3DShader.Vertex>($"{nameof(Geometry3DShader)}.slang", renderPass), IShaderFactory<Geometry3DShader>
{
    public override VkPipelineBindPoint BindPoint          => VkPipelineBindPoint.Graphics;
    public override VkFrontFace         FrontFace          => VkFrontFace.CounterClockwise;
    public override string              Name               => nameof(Geometry3DShader);
    public override VkPrimitiveTopology PrimitiveTopology  => VkPrimitiveTopology.TriangleList;

    public static Geometry3DShader Create(VkRenderPass renderPass) => new(renderPass);
}
