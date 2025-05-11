using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKRoundedRect(SKRect rect, float rx, float ry)
{
    public readonly SKRect Rect    = rect;
    public readonly float  RadiusX = rx;
    public readonly float  RadiusY = ry;

    public bool IsRounded => this.RadiusX > 0f || this.RadiusY > 0f;
}
