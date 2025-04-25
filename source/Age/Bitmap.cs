using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;

namespace Age;

public unsafe sealed class Bitmap : Disposable
{
    private readonly byte* buffer;
    private readonly int length;

    public int       BytesPerPixel { get; }
    public Size<uint> Size          { get; }

    public Bitmap(Size<uint> size, int bytesPerPixel)
    {
        this.length = (int)(bytesPerPixel * size.Height * size.Width);

        this.buffer        = (byte*)NativeMemory.AllocZeroed((uint)this.length);
        this.BytesPerPixel = bytesPerPixel;
        this.Size          = size;
    }

    public Span<byte> AsSpan() => new(this.buffer, this.length);

    protected override void OnDisposed(bool disposing) =>
        NativeMemory.Free(this.buffer);

    public uint[] GetPixels()
    {
        var buffer = new uint[this.Size.Width * this.Size.Height];

        this.AsSpan().CopyTo(this.BytesPerPixel, buffer, true);

        return buffer;
    }

    public static implicit operator Span<byte>(Bitmap bitmap) => bitmap.AsSpan();
}
