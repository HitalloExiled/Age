using System.Runtime.CompilerServices;

namespace Age.Core;

public abstract class SharedDisposable<T> : IDisposable
where T : SharedDisposable<T>
{
    public event Action? Disposed;

    private int disposedState;

    private int users = 1;

    public int Users => this.users;

    ~SharedDisposable() =>
        this.Dispose(false);

    public T Share()
    {
        this.ThrowIfDisposed();

        Interlocked.Increment(ref this.users);

        return Unsafe.As<T>(this);
    }

    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            var count = Interlocked.Decrement(ref this.users);

            if (count > 0)
            {
                return;
            }
        }

        if (Interlocked.CompareExchange(ref this.disposedState, 1, 0) == 0)
        {
            this.OnDisposed(disposing);
            Disposed?.Invoke();
        }
    }

    protected void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposedState == 1, this);

    protected abstract void OnDisposed(bool disposing);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
