using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public sealed class CanvasWireframeShader : Geometry2DShader, IShaderFactory<CanvasWireframeShader>
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    private CanvasWireframeShader(VkRenderPass renderPass)
    : base($"{nameof(CanvasWireframeShader)}.slang", renderPass) { }

    public static CanvasWireframeShader Create(VkRenderPass renderPass) =>
        new(renderPass);
}
