using Age.Numerics;
using SkiaSharp;

namespace Age.Extensions;

public static class Matrix3x2Extensions
{
    public static SKMatrix ToSKMatrix(this in Matrix3x2<float> matrix) =>
        new()
        {
            ScaleX = matrix.M11,
            SkewX  = matrix.M12,
            SkewY  = matrix.M21,
            ScaleY = matrix.M22,
            TransX = matrix.M31,
            TransY = matrix.M32,
            Persp0 = 0,
            Persp1 = 0,
            Persp2 = 1,
        };
}
