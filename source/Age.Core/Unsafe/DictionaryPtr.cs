using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Core.Unsafe;

public unsafe class DictionaryPtr<TKey, TValue> : IDisposable, IEnumerable<Ptr<TValue>>
where TKey : notnull
where TValue : unmanaged
{
    private readonly Dictionary<TKey, Ptr<TValue>> handlers = [];
    private bool disposed;

    public int Count => this.handlers.Count;

    ~DictionaryPtr() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Clear();

            this.disposed = true;
        }
    }

    public void Add(TKey key, in TValue value)
    {
        var handler = (TValue*)Marshal.AllocHGlobal(sizeof(TValue));

        *handler = value;

        this.handlers[key] = new(handler);
    }

    public void Clear()
    {
        foreach (var handler in this.handlers.Values)
        {
            Marshal.FreeHGlobal(handler);
        }

        this.handlers.Clear();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public TValue* Get(TKey key) =>
        this.handlers[key];

    public void Remove(TKey key)
    {
        var handler = this.handlers[key];

        Marshal.FreeHGlobal(handler);

        this.handlers.Remove(key);
    }

    public IEnumerator<Ptr<TValue>> GetEnumerator() =>
        this.handlers.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.handlers.GetEnumerator();
}
