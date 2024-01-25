namespace Age.Core.Interop;

public unsafe class StringArrayPtr(IList<string> source) : IDisposable
{
    public int    Length { get; }              = source.Count;
    public byte** PpData { get; private set; } = PointerHelper.Alloc(source);

    private bool disposed;

    ~StringArrayPtr() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            PointerHelper.Free(this.PpData, (uint)this.Length);
            this.PpData = null;

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string[] ToArray() =>
        PointerHelper.ToArray(this.PpData, (uint)this.Length);

    public static implicit operator byte**(StringArrayPtr value) => value.PpData;
}
