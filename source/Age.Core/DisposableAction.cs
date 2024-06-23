namespace Age.Core;

public class DisposableAction(Action action) : IDisposable
{
    private bool disposed;

    public void Dispose()
    {
        if (!this.disposed)
        {
            action.Invoke();
            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
