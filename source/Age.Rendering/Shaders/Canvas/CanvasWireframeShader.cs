using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public class CanvasWireframeShader : ShaderResources<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasWireframeShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;

    public CanvasWireframeShader() : base([$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasWireframeShader)}.frag"])
    { }
}
