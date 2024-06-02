using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public class GeometryShader : ShaderResources<GeometryShader.Vertex, GeometryShader.PushConstant>
{
    public override string              Name              { get; } = nameof(GeometryShader);
    public override VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;

    public GeometryShader() : base([$"Canvas/{nameof(GeometryShader)}.vert", $"Canvas/{nameof(GeometryShader)}.frag"])
    { }

    public struct Vertex(Vector3<float> position, Color color, in Vector2<float> texCoord) : IVertexInput
    {
        public Vector3<float> Position = position;
        public Color          Color    = color;
        public Vector2<float> TexCoord = texCoord;

        public static VkVertexInputAttributeDescription[] GetAttributes() => throw new NotImplementedException();
        public static VkVertexInputBindingDescription GetBindings() => throw new NotImplementedException();
    }

    public struct PushConstant : IPushConstant
    {
        public static uint               Offset { get; }
        public static uint               Size   { get; }
        public static VkShaderStageFlags Stages { get; }
    }
}
