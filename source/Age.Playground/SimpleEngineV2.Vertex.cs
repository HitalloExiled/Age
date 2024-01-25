using System.Runtime.InteropServices;
using Age.Numerics;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2
{
    private record struct Vertex(Vector3<float> Pos, Color Color, Vector2<float> TexCoord)
    {
        public Vector3<float> Pos      = Pos;
        public Color          Color    = Color;
        public Vector2<float> TexCoord = TexCoord;

        public static VertexInputBindingDescription GetBindingDescription()
        {
            var bindingDescription = new VertexInputBindingDescription
            {
                Binding   = 0,
                Stride    = (uint)Marshal.SizeOf<Vertex>(),
                InputRate = VertexInputRate.Vertex
            };

            return bindingDescription;
        }

        public static VertexInputAttributeDescription[] GetAttributeDescriptions()
        {
            var attributeDescriptions = new VertexInputAttributeDescription[]
            {
                new()
                {
                    Binding  = 0,
                    Location = 0,
                    Format   = Format.R32G32B32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Pos))!,
                },
                new()
                {
                    Binding  = 0,
                    Location = 1,
                    Format   = Format.R32G32B32A32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Color))!,
                },
                new()
                {
                    Binding  = 0,
                    Location = 2,
                    Format   = Format.R32G32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(TexCoord))!,
                }
            };

            return attributeDescriptions;
        }
    }
}
