using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public record struct Vertex : IVertexInput
    {
        public Point<float> Position = new();
        public Point<float> UV       = new();

        public Vertex() { }

        public Vertex(Point<float> position, Point<float> uv)
        {
            this.Position = position;
            this.UV       = uv;
        }

        public static VkVertexInputAttributeDescription[] GetAttributes() =>
            [
                new()
                {
                    Format   = VkFormat.R32G32Sfloat,
                    Location = 0,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
                },
                new()
                {
                    Format   = VkFormat.R32G32Sfloat,
                    Location = 1,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(UV)),
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