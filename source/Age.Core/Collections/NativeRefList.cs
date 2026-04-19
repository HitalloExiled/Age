using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeRefList<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeRefList))]
public unsafe ref partial struct NativeRefList<T> where T : unmanaged
{
    private bool disposed;

    private UnsafeListBuffer<T> unsafeBuffer;

    public readonly T this[uint index]
    {
        get => this.unsafeBuffer[(int)index];
        set => this.unsafeBuffer[(int)index] = value;
    }

    public T this[int index]
    {
        readonly get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer[index];
        }
        set
        {
            this.ThrowIfDisposed();

            this.unsafeBuffer[index] = value;
        }
    }

    public int Capacity
    {
        readonly get => this.unsafeBuffer.Capacity;
        set
        {
            this.ThrowIfDisposed();

            this.unsafeBuffer.Capacity = value;
        }
    }

    public readonly Span<T> this[Range range] => this.unsafeBuffer[range];

    public readonly T* Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public readonly int  Count   => this.unsafeBuffer.Count;
    public readonly bool IsEmpty => this.unsafeBuffer.IsEmpty;

    public NativeRefList(int capacity = 0) =>
        this.unsafeBuffer = new(capacity);

    public NativeRefList(ReadOnlySpan<T> values) =>
        this.unsafeBuffer = new(values);

    private readonly void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, typeof(NativeRefList<T>));

    public ref T Add()
    {
        this.ThrowIfDisposed();

        return ref this.unsafeBuffer.Add();
    }

    public void Add(T item)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Add(item);
    }

    public readonly Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.AsSpan();
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Clear();
    }

    public readonly bool Contains(T item) =>
        this.unsafeBuffer.Contains(item);

    public readonly void CopyTo(Span<T> items, int startIndex) =>
        this.unsafeBuffer.CopyTo(items, startIndex);

    public void EnsureCapacity(int capacity)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.EnsureCapacity(capacity);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.unsafeBuffer.Dispose();

        this.disposed = true;
    }

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        this.unsafeBuffer.GetEnumerator();

    public readonly int IndexOf(T item)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Insert(index, item);
    }

    public bool Remove(T item)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Remove(item);
    }

    public void RemoveAt(int index)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.RemoveAt(index);
    }

    public void RemoveAt(int startIndex, int count)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.RemoveAt(startIndex, count);
    }

    public readonly Span<T> Slice(int start, int length)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Slice(start, length);
    }

    public static implicit operator T*(NativeRefList<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeRefList<T> value) => value.AsSpan();
}
