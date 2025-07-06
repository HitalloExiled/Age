using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKCircle(SKPoint center, float radius)
{
    public readonly SKPoint Center = center;
    public readonly float   Radius = radius;
}
