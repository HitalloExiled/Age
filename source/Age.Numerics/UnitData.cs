using System.Runtime.InteropServices;

namespace Age.Numerics;

[StructLayout(LayoutKind.Explicit)]
internal struct UnitData
{
    [FieldOffset(0)]
    public int Pixel;

    [FieldOffset(0)]
    public float Percentage;

    [FieldOffset(0)]
    public float Em;
}
