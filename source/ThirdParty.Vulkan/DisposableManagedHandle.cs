namespace ThirdParty.Vulkan;

public abstract class DisposableManagedHandle<T> : ManagedHandle<T>, IDisposable where T : ManagedHandle<T>
{
    private bool disposed;

    internal DisposableManagedHandle() : base() { }
    internal DisposableManagedHandle(VkHandle<T> handle) : base(handle) { }

    protected abstract void Disposed();

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Disposed();
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
