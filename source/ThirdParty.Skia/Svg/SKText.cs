using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal class SKText(SKPoint location, SKTextAlign textAlign)
{
    private readonly List<SKTextSpan> spans = [];

    public SKPoint     Location  { get; } = location;
    public SKTextAlign TextAlign { get; } = textAlign;

    public IReadOnlyList<SKTextSpan> Spans => this.spans;

    public void Append(SKTextSpan span) =>
        this.spans.Add(span);

    public float MeasureTextWidth()
    {
        var width = 0f;

        foreach (var span in this.spans)
        {
            width += span.TextWidth;
        }

        return width;
    }
}
