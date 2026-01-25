using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public abstract partial class Geometry2DShader(string file, VkRenderPass renderPass) : Shader<Geometry2DShader.Vertex>(file, renderPass)
{
    public override VkPipelineBindPoint BindPoint         => VkPipelineBindPoint.Graphics;
    public override VkFrontFace         FrontFace         => VkFrontFace.Clockwise;
    public override string              Name              => nameof(Geometry2DShader);
    public override VkPrimitiveTopology PrimitiveTopology => VkPrimitiveTopology.TriangleList;
}
