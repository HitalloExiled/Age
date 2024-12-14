using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class GeometryShader
{
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
}
