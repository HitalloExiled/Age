using Age.Numerics;
using SkiaSharp;

namespace Age.Rendering;

public record Glyph
{
    public required int         Id   { get; init; }
    public required Size<float> Size { get; init; }
    public SKFontMetrics Metrics { get; internal set; }
}
