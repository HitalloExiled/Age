using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeRefList<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeRefList))]
public unsafe ref partial struct NativeRefList<T> where T : unmanaged
{
    private bool disposed;

    private UnsafeListBuffer<T> unsefeBuffer;

    public readonly T this[uint index]
    {
        get => this.unsefeBuffer[(int)index];
        set => this.unsefeBuffer[(int)index] = value;
    }

    public T this[int index]
    {
        readonly get
        {
            this.ThrowIfDisposed();

            return this.unsefeBuffer[index];
        }
        set
        {
            this.ThrowIfDisposed();

            this.unsefeBuffer[index] = value;
        }
    }

    public int Capacity
    {
        readonly get => this.unsefeBuffer.Capacity;
        set
        {
            this.ThrowIfDisposed();

            this.unsefeBuffer.Capacity = value;
        }
    }

    public readonly Span<T> this[Range range] => this.unsefeBuffer[range];

    public readonly T* Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsefeBuffer.Buffer;
        }
    }

    public readonly int  Count   => this.unsefeBuffer.Count;
    public readonly bool IsEmpty => this.unsefeBuffer.IsEmpty;

    public NativeRefList(int capacity = 0) =>
        this.unsefeBuffer = new(capacity);

    public NativeRefList(ReadOnlySpan<T> values) =>
        this.unsefeBuffer = new(values);

    private readonly void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, typeof(NativeRefList<T>));

    public ref T Add()
    {
        this.ThrowIfDisposed();

        return ref this.unsefeBuffer.Add();
    }

    public void Add(T item)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.Add(item);
    }

    public readonly Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.AsSpan();
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.Clear();
    }

    public readonly bool Contains(T item) =>
        this.unsefeBuffer.Contains(item);

    public readonly void CopyTo(Span<T> items, int startIndex) =>
        this.unsefeBuffer.CopyTo(items, startIndex);

    public void EnsureCapacity(int capacity)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.EnsureCapacity(capacity);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.unsefeBuffer.Dispose();

        this.disposed = true;
    }

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        this.unsefeBuffer.GetEnumerator();

    public readonly int IndexOf(T item)
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.Insert(index, item);
    }

    public bool Remove(T item)
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.Remove(item);
    }

    public void RemoveAt(int index)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.RemoveAt(index);
    }

    public void RemoveAt(int startIndex, int count)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.RemoveAt(startIndex, count);
    }

    public readonly Span<T> Slice(int start, int length)
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.Slice(start, length);
    }

    public static implicit operator T*(NativeRefList<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeRefList<T> value) => value.AsSpan();
}
