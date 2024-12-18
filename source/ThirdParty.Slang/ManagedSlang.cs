namespace ThirdParty.Slang;

public abstract class ManagedSlang<T>
where T : ManagedSlang<T>
{
    internal Handle<T> Handle { get; }

    internal ManagedSlang(Handle<T> handle)
    {
        if (handle.Value == default)
        {
            throw new InvalidOperationException();
        }

        this.Handle = handle;
    }

    public static bool operator == (ManagedSlang<T> left, ManagedSlang<T> right) => left.Handle == right.Handle;
    public static bool operator != (ManagedSlang<T> left, ManagedSlang<T> right) => left.Handle != right.Handle;

    public override bool Equals(object? obj) =>
        obj is ManagedSlang<T> managed && managed == this;

    public override int GetHashCode() =>
        this.Handle.GetHashCode();
}

public abstract class DisposableManagedSlang<T> : ManagedSlang<T>, IDisposable
where T : DisposableManagedSlang<T>
{
    private bool disposed;

    internal DisposableManagedSlang(Handle<T> handle) : base(handle)
    { }

    ~DisposableManagedSlang() =>
        this.Dispose(false);

    protected void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Disposed(disposing);
            this.disposed = true;
        }
    }

    protected abstract void Disposed(bool disposing);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
