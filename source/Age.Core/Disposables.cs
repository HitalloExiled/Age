using System.Collections;

namespace Age.Core;

public class Disposables : IDisposable, IEnumerable<IDisposable>
{
    private readonly List<IDisposable> disposables = [];
    private bool disposed;

    public Disposables() { }

    public Disposables(params IDisposable[] disposables) =>
        this.disposables.AddRange(disposables);

    ~Disposables() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Add(IDisposable disposable) =>
        this.disposables.Add(disposable);

    public void Remove(IDisposable disposable) =>
        this.disposables.Remove(disposable);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IEnumerator<IDisposable> GetEnumerator() =>
        ((IEnumerable<IDisposable>)this.disposables).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)this.disposables).GetEnumerator();
}
