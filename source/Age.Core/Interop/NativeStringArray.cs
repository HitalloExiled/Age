using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Interop;

public unsafe class NativeStringArray : IDisposable
{
    public int    Length { get; }
    public byte** PpData { get; private set; }

    private bool disposed;

    public NativeStringArray(IList<string> source)
    {
        var ppData = (byte**)NativeMemory.Alloc((uint)(sizeof(byte*) * source.Count));

        for (var i = 0; i < source.Count; i++)
        {
            var bytes = Encoding.UTF8.GetBytes(source[i]);

            var pData = (byte*)NativeMemory.Alloc((uint)(sizeof(byte) * bytes.Length + 1));

            pData[bytes.Length] = 0;

            bytes.AsSpan().CopyTo(new Span<byte>(pData, bytes.Length));

            ppData[i] = pData;
        }

        this.PpData = ppData;
        this.Length = source.Count;
    }

    ~NativeStringArray() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            for (var i = 0; i < this.Length; i++)
            {
                NativeMemory.Free(this.PpData[i]);
            }

            NativeMemory.Free(this.PpData);

            this.PpData = null;

            this.disposed = true;
        }
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

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string[] ToArray() =>
        ToArray(this.PpData, (uint)this.Length);

    public static implicit operator byte**(NativeStringArray value) => value.PpData;
}
