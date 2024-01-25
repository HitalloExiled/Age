namespace ThirdParty.Vulkan;

public abstract class DisposableNativeHandle : NativeHandle, IDisposable
{
    private bool disposed;

    internal DisposableNativeHandle() : base() { }
    internal DisposableNativeHandle(nint handle) : base(handle) { }

    ~DisposableNativeHandle() => this.Dispose(false);

    protected abstract void OnDispose();

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.OnDispose();
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
