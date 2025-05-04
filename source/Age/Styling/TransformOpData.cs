using System.Runtime.InteropServices;
using Age.Numerics;

namespace Age.Styling;

[StructLayout(LayoutKind.Explicit)]
internal struct TransformOpData
{
    [FieldOffset(0)] public PointUnit        Translation;
    [FieldOffset(0)] public float            Rotation;
    [FieldOffset(0)] public Point<float>     Scale;
    [FieldOffset(0)] public Point<float>     Skew;
    [FieldOffset(0)] public Matrix3x2<float> Matrix;
}
