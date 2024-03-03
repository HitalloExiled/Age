using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders;

public class WireframeShader : ShaderResources<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(WireframeShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    public WireframeShader() : base([$"{nameof(CanvasShader)}.vert", $"{nameof(WireframeShader)}.frag"])
    { }
}
