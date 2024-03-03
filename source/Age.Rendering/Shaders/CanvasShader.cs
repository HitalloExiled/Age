using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders;

public partial class CanvasShader : ShaderResources<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasShader() : base([$"{nameof(CanvasShader)}.vert", $"{nameof(CanvasShader)}.frag"])
    { }
}
