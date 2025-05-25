using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal class SKText(SKPoint location, SKTextAlign textAlign) : IDisposable
{
    private readonly List<SKTextSpan> spans = [];

    private bool disposed;

    public SKPoint     Location  { get; } = location;
    public SKTextAlign TextAlign { get; } = textAlign;

    public IReadOnlyList<SKTextSpan> Spans => this.spans;

    public void Append(SKTextSpan span) =>
        this.spans.Add(span);

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            foreach (var span in this.spans)
            {
                span.Font.Dispose();
                span.Fill?.Dispose();
                span.Stroke?.Dispose();
                span.TextPath?.Path.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }

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
