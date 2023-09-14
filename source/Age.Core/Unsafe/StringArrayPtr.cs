using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Unsafe;

public unsafe class StringArrayPtr : IDisposable
{
    public int    Length  { get; }
    public int    MaxSize { get; }
    public byte** PpData  { get; private set; }

    private bool disposed;

    ~StringArrayPtr() =>
        this.Dispose(false);

    public StringArrayPtr(IList<string> values, int maxSize = 256)
    {
        var ppData = (byte**)Marshal.AllocHGlobal(sizeof(byte*) * values.Count);

        for (var i = 0; i < values.Count; i++)
        {
            var bytes  = values[i].ToUTF8Bytes();

            if (bytes.Length > maxSize)
            {
                throw new InvalidOperationException($"Element length at {i} is greater than the max size {maxSize}");
            }

            var pValue = (byte*)Marshal.AllocHGlobal(sizeof(byte) * maxSize);

            UnmanagedUtils.ZeroFill(pValue, maxSize);

            UnmanagedUtils.Copy(bytes, pValue, bytes.Length);

            ppData[i] = pValue;
        }

        this.PpData  = ppData;
        this.Length  = values.Count;
        this.MaxSize = maxSize;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            for (var i = 0; i < this.Length; i++)
            {
                Marshal.FreeHGlobal((nint)this.PpData[i]);
                this.PpData[i] = null;
            }

            Marshal.FreeHGlobal((nint)this.PpData);
            this.PpData   = null;
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string[] ToArray()
    {
        var result = new string[this.Length];

        for (var i = 0; i < this.Length; i++)
        {
            result[i] = Marshal.PtrToStringAnsi((nint)this.PpData[i])!;
        }

        return result;
    }

    public static implicit operator byte**(StringArrayPtr value) => value.PpData;
}
