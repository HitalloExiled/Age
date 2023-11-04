using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

namespace Age.Playground;

public unsafe partial class SimpleEngine
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
                binding   = 0,
                stride    = (uint)Marshal.SizeOf<Vertex>(),
                inputRate = VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX
            };

            return bindingDescription;
        }

        public static VkVertexInputAttributeDescription[] GetAttributeDescriptions()
        {
            var attributeDescriptions = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    binding  = 0,
                    location = 0,
                    format   = VkFormat.VK_FORMAT_R32G32B32_SFLOAT,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Pos))!,
                },
                new()
                {
                    binding  = 0,
                    location = 1,
                    format   = VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Color))!,
                },
                new()
                {
                    binding  = 0,
                    location = 2,
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(TexCoord))!,
                }
            };

            return attributeDescriptions;
        }
    }
}
