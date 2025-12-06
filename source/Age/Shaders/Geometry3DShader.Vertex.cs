using System.Runtime.InteropServices;
using Age.Core.Collections;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class Geometry3DShader
{
    public struct Vertex(Vector3<float> position, Color color, in Vector2<float> uv) : IVertexInput
    {
        public Vector3<float> Position = position;
        public Color          Color    = color;
        public Vector2<float> UV       = uv;

        public static RefArray<VkVertexInputAttributeDescription> GetAttributes()
        {
            var attributes = new RefArray<VkVertexInputAttributeDescription>(3);

            attributes[0] = new()
            {
                Binding  = 0,
                Location = 0,
                Format   = VkFormat.R32G32B32Sfloat,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
            };

            attributes[1] = new()
            {
                Binding  = 0,
                Location = 1,
                Format   = VkFormat.R32G32B32A32Sfloat,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Color)),
            };

            attributes[2] = new()
            {
                Binding  = 0,
                Location = 2,
                Format   = VkFormat.R32G32Sfloat,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(UV)),
            };

            return attributes;
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
