using System.Runtime.InteropServices;
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

    public GeometryShader() : base([$"{nameof(GeometryShader)}.vert", $"{nameof(GeometryShader)}.frag"])
    { }

    public struct Vertex(Vector3<float> position, Color color, in Vector2<float> texCoord) : IVertexInput
    {
        public Vector3<float> Position = position;
        public Color          Color    = color;
        public Vector2<float> TexCoord = texCoord;

        public static VkVertexInputAttributeDescription[] GetAttributes()
        {
            var attributeDescriptions = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    Binding  = 0,
                    Location = 0,
                    Format   = VkFormat.R32G32B32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position))!,
                },
                new()
                {
                    Binding  = 0,
                    Location = 1,
                    Format   = VkFormat.R32G32B32A32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Color))!,
                },
                new()
                {
                    Binding  = 0,
                    Location = 2,
                    Format   = VkFormat.R32G32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(TexCoord))!,
                }
            };

            return attributeDescriptions;
        }

        public static VkVertexInputBindingDescription GetBindings() =>
            new()
            {
                Binding   = 0,
                Stride    = (uint)Marshal.SizeOf<Vertex>(),
                InputRate = VkVertexInputRate.Vertex
            };
    }

    public readonly struct PushConstant : IPushConstant
    {
        public static uint               Offset { get; }
        public static uint               Size   { get; }
        public static VkShaderStageFlags Stages { get; }
    }
}
