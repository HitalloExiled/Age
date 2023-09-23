using System.Runtime.InteropServices;
using Age.Core.Math;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

namespace Age;

#pragma warning disable IDE1006
public unsafe partial class SimpleEngine
{
    private struct Vertex(Vector2<float> pos, Vector3<float> color)
    {
        public Vector2<float> pos   = pos;
        public Vector3<float> color = color;

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
                    offset   = (uint)Marshal.OffsetOf<Vertex>("pos")!,
                },
                new()
                {
                    binding  = 0,
                    location = 1,
                    format   = VkFormat.VK_FORMAT_R32G32B32_SFLOAT,
                    offset   = (uint)Marshal.OffsetOf<Vertex>("color")!,
                }
            };

            return attributeDescriptions;
        }
    }
}
