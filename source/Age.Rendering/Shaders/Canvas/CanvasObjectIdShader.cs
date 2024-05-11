using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasObjectIdShader : ShaderResources<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasObjectIdShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasObjectIdShader() : base([$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasObjectIdShader)}.frag"])
    { }
}
