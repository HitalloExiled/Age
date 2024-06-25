using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders;

public partial class GeometryShader : Shader<GeometryShader.Vertex, GeometryShader.PushConstant>
{
    public override string              Name              { get; } = nameof(GeometryShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public GeometryShader() : base([$"{nameof(GeometryShader)}.vert", $"{nameof(GeometryShader)}.frag"])
    { }
}
