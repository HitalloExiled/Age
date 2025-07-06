using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal record SkTextSpanPath(SKPath Path, string? LengthAdjust, string? Method, string? Spacing, float? StartOffset, float? TextLength);
