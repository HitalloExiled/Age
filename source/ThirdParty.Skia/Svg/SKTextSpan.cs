using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKTextSpan(string text, SKFont font, SKPaint? fill, SKPaint? stroke, float? x, float? y, float baselineShift, SkTextSpanPath? textPath)
{
    public readonly string          Text     = text;
    public readonly SKFont          Font     = font;
    public readonly SKPaint?        Fill     = fill;
    public readonly SKPaint?        Stroke   = stroke;
    public readonly SkTextSpanPath? TextPath = textPath;

    public readonly float? X             = x;
    public readonly float? Y             = y;
    public readonly float  BaselineShift = baselineShift;

    public float TextWidth => this.Font.MeasureText(this.Text);
}
