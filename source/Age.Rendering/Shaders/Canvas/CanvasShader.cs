using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader : Shader<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public override string              Name              { get; } = nameof(CanvasShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public CanvasShader() : base([$"Canvas/{nameof(CanvasShader)}.vert", $"Canvas/{nameof(CanvasShader)}.frag"])
    { }
}
