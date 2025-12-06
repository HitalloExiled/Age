using System.Runtime.InteropServices;
using Age.Core.Collections;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class Geometry2DShader
{
    public record struct Vertex : IVertexInput
    {
        public Point<float> Position = new();

        public Vertex() { }

        public Vertex(float x, float y) =>
            this.Position = new(x, y);

        public Vertex(Point<float> position) =>
            this.Position = position;

        public static RefArray<VkVertexInputAttributeDescription> GetAttributes()
        {
            var attributes = new RefArray<VkVertexInputAttributeDescription>(1);

            attributes[0] = new()
            {
                Format   = VkFormat.R32G32Sfloat,
                Location = 0,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
            };

            return attributes;
        }

        public unsafe static VkVertexInputBindingDescription GetBindings() =>
            new()
            {
                Stride    = (uint)sizeof(Vertex),
                InputRate = VkVertexInputRate.Vertex,
            };
    };
}
