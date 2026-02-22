using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public abstract partial class Geometry3DShader(string file, VkRenderPass renderPass) : Shader<Geometry3DShader.Vertex>(file, renderPass)
{
    public override VkPipelineBindPoint BindPoint         => VkPipelineBindPoint.Graphics;
    public override VkFrontFace         FrontFace         => VkFrontFace.CounterClockwise;
    public override VkPrimitiveTopology PrimitiveTopology => VkPrimitiveTopology.TriangleList;
}
