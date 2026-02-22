using Age.Numerics;
using System.Runtime.InteropServices;

namespace Age.Rendering.Resources;

[StructLayout(LayoutKind.Explicit)]
public struct ClearValue
{
    [FieldOffset(0)]
    public Color Color;

    [FieldOffset(0)]
    public ClearDepthStencilValue DepthStencil;

    public ClearValue(in Color color) =>
        this.Color = color;

    public ClearValue(float depth, uint stencil)
    {
        this.DepthStencil = new()
        {
            Depth   = depth,
            Stencil = stencil
        };
    }
}
