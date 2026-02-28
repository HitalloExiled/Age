using Age.Core.Collections;
using SkiaSharp;

namespace ThirdParty.Skia.Svg;

internal readonly ref struct SKSvgImage(SKRect rect, ReadOnlySpan<char> uri, NativeRefArray<byte> bytes = default) : IDisposable
{
    public readonly SKRect             Rect  = rect;
    public readonly ReadOnlySpan<char> Uri   = uri;
    public readonly NativeRefArray<byte>     Bytes = bytes;

    public void Dispose() =>
        this.Bytes.Dispose();
}
