using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering;

public record struct Vertex : IVertexInput
{
    public Point<float> Position = new();
    public Color        Color    = new();
    public Point<float> UV       = new();

    public Vertex() { }

    public Vertex(Point<float> position, Color color, Point<float> uv)
    {
        this.Position = position;
        this.Color    = color;
        this.UV       = uv;
    }

    public static VkVertexInputAttributeDescription[] GetAttributes() =>
        [
            new()
            {
                Binding  = 0,
                Format   = VkFormat.R32G32Sfloat,
                Location = 0,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
            },
            new()
            {
                Binding  = 0,
                Format   = VkFormat.R32G32B32A32Sfloat,
                Location = 1,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Color)),
            },
            new()
            {
                Binding  = 0,
                Format   = VkFormat.R32G32Sfloat,
                Location = 2,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(UV)),
            },
        ];

    public unsafe static VkVertexInputBindingDescription GetBindings() =>
        new()
        {
            Binding   = 0,
            Stride    = (uint)sizeof(Vertex),
            InputRate = VkVertexInputRate.Vertex,
        };
};
