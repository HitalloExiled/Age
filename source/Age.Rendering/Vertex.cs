using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering;

public record struct Vertex : IVertexInput
{
    public Vector3<float> Position = new();
    public Point<float>   UV       = new();

    public Vertex() { }

    public Vertex(Vector3<float> position, Point<float> uv)
    {
        this.Position = position;
        this.UV       = uv;
    }

    public static VkVertexInputAttributeDescription[] GetAttributes() =>
        [
            new()
            {
                Binding  = 0,
                Format   = VkFormat.R32G32B32A32Sfloat,
                Location = 0,
                Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Position)),
            },
            new()
            {
                Binding  = 0,
                Format   = VkFormat.R32G32Sfloat,
                Location = 1,
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
