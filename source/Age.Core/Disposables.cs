using System.Collections;

namespace Age.Core;

public sealed class Disposables : IDisposable, IEnumerable<IDisposable>
{
    private readonly List<IDisposable> disposables = [];
    private bool disposed;

    public Disposables() { }

    public Disposables(params ReadOnlySpan<IDisposable> disposables) =>
        this.disposables.AddRange(disposables);

    ~Disposables() => this.Dispose();

    public void Add(IDisposable disposable) =>
        this.disposables.Add(disposable);

    public void Remove(IDisposable disposable) =>
        this.disposables.Remove(disposable);

    public void Dispose()
    {
        if (!this.disposed)
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    public IEnumerator<IDisposable> GetEnumerator() =>
        this.disposables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
