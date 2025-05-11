using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKLine(SKPoint p1, SKPoint p2)
{
    public readonly SKPoint P1 = p1;
    public readonly SKPoint P2 = p2;
}
