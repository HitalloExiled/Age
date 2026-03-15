using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeList<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeList))]
public unsafe partial class NativeList<T> : Disposable, IEnumerable<T> where T : unmanaged
{
    private UnsafeListBuffer<T> unsefeBuffer;

    public T this[uint index]
    {
        get => this.unsefeBuffer[(int)index];
        set => this.unsefeBuffer[(int)index] = value;
    }

    public T this[int index]
    {
        get
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
        get => this.unsefeBuffer.Capacity;
        set
        {
            this.ThrowIfDisposed();

            this.unsefeBuffer.Capacity = value;
        }
    }

    public Span<T> this[Range range] => this.unsefeBuffer[range];

    public T* Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsefeBuffer.Buffer;
        }
    }

    public int  Count   => this.unsefeBuffer.Count;
    public bool IsEmpty => this.unsefeBuffer.IsEmpty;

    public NativeList(int capacity = 0) =>
        this.unsefeBuffer = new(capacity);

    public NativeList(ReadOnlySpan<T> values) =>
        this.unsefeBuffer = new(values);

    protected override void OnDisposed(bool disposing) =>
        this.unsefeBuffer.Dispose();

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.GetEnumerator();
    }

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

    public Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.AsSpan();
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.Clear();
    }

    public bool Contains(T item) =>
        this.unsefeBuffer.Contains(item);

    public void CopyTo(Span<T> items, int startIndex) =>
        this.unsefeBuffer.CopyTo(items, startIndex);

    public void EnsureCapacity(int capacity)
    {
        this.ThrowIfDisposed();

        this.unsefeBuffer.EnsureCapacity(capacity);
    }

    public UnsafeEnumerator<T> GetEnumerator() =>
        this.unsefeBuffer.GetEnumerator();

    public int IndexOf(T item)
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.IndexOf(item);
    }

    public void Insert(int index, in T item)
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

    public Span<T> Slice(int start, int length)
    {
        this.ThrowIfDisposed();

        return this.unsefeBuffer.Slice(start, length);
    }

    public static implicit operator T*(NativeList<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeList<T> value) => value.AsSpan();
}
