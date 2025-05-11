using System.Collections;
using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal class SKText(SKPoint location, SKTextAlign textAlign) : IEnumerable<SKTextSpan>, IEnumerable
{
    private readonly List<SKTextSpan> spans = [];

    public SKPoint     Location  { get; } = location;
    public SKTextAlign TextAlign { get; } = textAlign;

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    IEnumerator<SKTextSpan> IEnumerable<SKTextSpan>.GetEnumerator() => this.GetEnumerator();

    public void Append(SKTextSpan span) =>
        this.spans.Add(span);

    public IEnumerator<SKTextSpan> GetEnumerator() =>
        this.spans.GetEnumerator();

    public float MeasureTextWidth()
    {
        var width = 0f;

        foreach (var span in this.spans)
        {
            width += span.MeasureTextWidth();
        }

        return width;
    }
}
