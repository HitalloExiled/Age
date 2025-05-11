using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal class SKTextSpan(string text, SKPaint? fill, float? x = null, float? y = null, float? baselineShift = null)
{
    public string   Text          { get; } = text;
    public SKPaint? Fill          { get; } = fill;
    public float?   X             { get; } = x;
    public float?   Y             { get; } = y;
    public float?   BaselineShift { get; } = baselineShift;

#pragma warning disable CS0618 // Type or member is obsolete
    public float MeasureTextWidth() => this.Fill?.MeasureText(this.Text) ?? 0;
#pragma warning restore CS0618 // Type or member is obsolete
}
