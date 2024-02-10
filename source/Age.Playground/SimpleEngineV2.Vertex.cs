using System.Runtime.InteropServices;
using Age.Numerics;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2
{
    private record struct Vertex(Vector3<float> Pos, Color Color, Vector2<float> TexCoord)
    {
        public Vector3<float> Pos      = Pos;
        public Color          Color    = Color;
        public Vector2<float> TexCoord = TexCoord;

        public static VkVertexInputBindingDescription GetBindingDescription()
        {
            var bindingDescription = new VkVertexInputBindingDescription
            {
                Binding   = 0,
                Stride    = (uint)Marshal.SizeOf<Vertex>(),
                InputRate = VkVertexInputRate.Vertex
            };

            return bindingDescription;
        }

        public static VkVertexInputAttributeDescription[] GetAttributeDescriptions()
        {
            var attributeDescriptions = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    Binding  = 0,
                    Location = 0,
                    Format   = VkFormat.R32G32B32Sfloat,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Pos))!,
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
    }
}
