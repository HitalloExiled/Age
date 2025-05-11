using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly struct SKTextSpan(string text, SKFont font, float? x, float? y, float baselineShift)
{
    public readonly string Text = text;
    public readonly SKFont Font = font;

    public readonly float? X             = x;
    public readonly float? Y             = y;
    public readonly float  BaselineShift = baselineShift;

    public float TextWidth => this.Font.MeasureText(this.Text);
}
