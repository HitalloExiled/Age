using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Shaders;

public partial class CanvasShader
{
    public record struct Vertex : IVertexInput
    {
        public Point<float> Position = new();

        public Vertex() { }

        public Vertex(float x, float y) =>
            this.Position = new(x, y);

        public Vertex(Point<float> position) =>
            this.Position = position;

        public static VkVertexInputAttributeDescription[] GetAttributes() =>
            [
                new()
                {
                    Format   = VkFormat.R32G32Sfloat,
                    Location = 0,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
                },
            ];

        public unsafe static VkVertexInputBindingDescription GetBindings() =>
            new()
            {
                Stride    = (uint)sizeof(Vertex),
                InputRate = VkVertexInputRate.Vertex,
            };
    };
}
