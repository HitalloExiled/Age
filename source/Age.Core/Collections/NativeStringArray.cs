using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Collections;

public unsafe class NativeStringArray : Disposable
{
    private byte** buffer;

    public int Length { get; }

    public NativeStringArray(ReadOnlySpan<string> source)
    {
        var buffer = (byte**)NativeMemory.Alloc((uint)(sizeof(byte*) * source.Length));

        for (var i = 0; i < source.Length; i++)
        {
            var bytes = Encoding.UTF8.GetBytes(source[i]);

            var pData = (byte*)NativeMemory.Alloc((uint)((sizeof(byte) * bytes.Length) + 1));

            pData[bytes.Length] = 0;

            bytes.AsSpan().CopyTo(new Span<byte>(pData, bytes.Length));

            buffer[i] = pData;
        }

        this.buffer = buffer;
        this.Length = source.Length;
    }

    protected override void OnDisposed(bool disposing)
    {
        for (var i = 0; i < this.Length; i++)
        {
            NativeMemory.Free(this.buffer[i]);
        }

        NativeMemory.Free(this.buffer);

        this.buffer = null;
    }

    public static string[] ToArray(byte** ppSource, uint length)
    {
        var result = new string[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = Marshal.PtrToStringAnsi((nint)ppSource[i])!;
        }

        return result;
    }

    public byte** AsPointer() => this.buffer;

    public string[] ToArray() =>
        ToArray(this.buffer, (uint)this.Length);

    public static implicit operator byte**(NativeStringArray value) => value.buffer;
}
