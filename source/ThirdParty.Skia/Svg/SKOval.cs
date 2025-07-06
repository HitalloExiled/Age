using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKOval(SKPoint center, float rx, float ry)
{
    public readonly SKPoint Center  = center;
    public readonly float   RadiusX = rx;
    public readonly float   RadiusY = ry;

    public SKRect BoundingRect => new(this.Center.X - this.RadiusX, this.Center.Y - this.RadiusY, this.Center.X + this.RadiusX, this.Center.Y + this.RadiusY);
}
