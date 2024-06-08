using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasIndexShader : ShaderResources<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasIndexShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasIndexShader() : base([$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasIndexShader)}.frag"])
    { }
}
