using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Unsafe;

public unsafe class StringArrayPtr : IDisposable
{
    public int    Length  { get; }
    public byte** PpData  { get; private set; }

    private bool disposed;

    ~StringArrayPtr() =>
        this.Dispose(false);

    public StringArrayPtr(IList<string> values)
    {
        var ppData = (byte**)Marshal.AllocHGlobal(sizeof(byte*) * values.Count);

        for (var i = 0; i < values.Count; i++)
        {
            var bytes = Encoding.UTF8.GetBytes(values[i]);

            var pValue = (byte*)Marshal.AllocHGlobal(bytes.Length + 1);

            UnmanagedUtils.Copy(bytes, pValue, bytes.Length);

            pValue[bytes.Length] = 0;

            ppData[i] = pValue;
        }

        this.PpData  = ppData;
        this.Length  = values.Count;
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
