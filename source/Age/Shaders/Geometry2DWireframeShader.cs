using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasWireframeShader(VkRenderPass renderPass, in ShaderOptions shaderOptions)
: Geometry2DShader($"{nameof(CanvasWireframeShader)}.slang", renderPass, shaderOptions)
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    public CanvasWireframeShader(VkRenderPass renderPass, bool watch)
    :  this(renderPass, new ShaderOptions() { Watch = watch }) { }
}
