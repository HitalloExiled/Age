namespace Age.Core;

public class DisposableAction(Action action) : IDisposable
{
    public bool disposed;

    public void Dispose()
    {
        if (!disposed)
        {
            action.Invoke();
            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
