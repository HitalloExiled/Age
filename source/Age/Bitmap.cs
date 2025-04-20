using System.Runtime.InteropServices;
using Age.Core;
using Age.Numerics;

namespace Age;

public unsafe sealed class Bitmap : Disposable
{
    private readonly byte* buffer;
    private readonly int length;
    public ColorMode  ColorMode { get; }
    public Size<uint> Size      { get; }

    public ushort BytesPerPixel => (ushort)this.ColorMode;

    public Bitmap(Size<uint> size, ColorMode colorMode)
    {
        this.length = (int)((int)colorMode * size.Height * size.Width);

        this.buffer    = (byte*)NativeMemory.AllocZeroed((uint)this.length);
        this.ColorMode = colorMode;
        this.Size      = size;
    }

    public Span<byte> AsSpan() => new(this.buffer, this.length);

    protected override void OnDisposed(bool disposing) =>
        NativeMemory.Free(this.buffer);

    public static implicit operator Span<byte>(Bitmap bitmap) => bitmap.AsSpan();
}
