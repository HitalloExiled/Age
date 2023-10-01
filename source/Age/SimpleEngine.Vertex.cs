using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

namespace Age;

public unsafe partial class SimpleEngine
{
    private struct Vertex(Vector2<float> pos, Vector3<float> color, Vector2<float> texCoord)
    {
        public Vector2<float> Pos      = pos;
        public Vector3<float> Color    = color;
        public Vector2<float> TexCoord = texCoord;

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
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Pos))!,
                },
                new()
                {
                    binding  = 0,
                    location = 1,
                    format   = VkFormat.VK_FORMAT_R32G32B32_SFLOAT,
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
