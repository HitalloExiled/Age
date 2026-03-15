using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeArray<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeArray))]
public unsafe partial class NativeArray<T> : Disposable, IEnumerable<T> where T : unmanaged
{
    private UnsafeArrayBuffer<T> unsafeBuffer;

    public T this[uint index]
    {
        get
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

    public T this[int index]
    {
        get
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

    public Span<T> this[Range range]
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer[range];
        }
    }

    public T* Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public bool IsEmpty => this.unsafeBuffer.IsEmpty;
    public int Length   => this.unsafeBuffer.Length;

    public NativeArray(int size) =>
        this.unsafeBuffer = new(size);

    public NativeArray(uint size) =>
        this.unsafeBuffer = new(size);

    public NativeArray(ReadOnlySpan<T> values) =>
        this.unsafeBuffer = new(values);

    protected override void OnDisposed(bool disposing) =>
        this.unsafeBuffer.Dispose();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.unsafeBuffer.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        this.unsafeBuffer.GetEnumerator();

    public Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.AsSpan();
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Clear();
    }

    public bool Contains(T item)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Contains(item);
    }

    public void CopyTo(Span<T> span)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.CopyTo(span);
    }

    public void CopyTo(Span<T> array, int startIndex)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.CopyTo(array, startIndex);
    }

    public UnsafeEnumerator<T> GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.IndexOf(item);
    }

    public void Resize(int size)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Resize(size);
    }

    public void ResizeCopy(ReadOnlySpan<T> source)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.ResizeCopy(source);
    }

    public Span<T> Slice(int start, int length)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Slice(start, length);
    }

    public static implicit operator T*(NativeArray<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeArray<T> value) => value.AsSpan();
}
