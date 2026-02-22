using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal sealed record SkTextSpanPath(SKPath Path, string? LengthAdjust, string? Method, string? Spacing, float? StartOffset, float? TextLength);
