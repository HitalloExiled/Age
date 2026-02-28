using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeRefArray<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeRefArray))]
public unsafe ref partial struct NativeRefArray<T> where T : unmanaged
{
    private bool disposed;

    private UnsafeArrayBuffer<T> unsafeBuffer;

    public readonly T this[uint index]
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

    public readonly T this[int index]
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

    public readonly Span<T> this[Range range]
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer[range];
        }
    }

    public readonly T* Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public readonly bool IsEmpty => this.unsafeBuffer.IsEmpty;
    public readonly int  Length  => this.unsafeBuffer.Length;

    public NativeRefArray(int size) =>
        this.unsafeBuffer = new(size);

    public NativeRefArray(uint size) =>
        this.unsafeBuffer = new(size);

    public NativeRefArray(ReadOnlySpan<T> values) =>
        this.unsafeBuffer = new(values);

    private readonly void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, typeof(NativeRefArray<T>));

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.unsafeBuffer.Dispose();

        this.disposed = true;
    }

    public readonly Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.AsSpan();
    }

    public readonly void Clear()
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Clear();
    }

    public readonly bool Contains(T item)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Contains(item);
    }

    public readonly void CopyTo(Span<T> span)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.CopyTo(span);
    }

    public readonly void CopyTo(Span<T> array, int startIndex)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.CopyTo(array, startIndex);
    }

    public readonly UnsafeEnumerator<T> GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public readonly int IndexOf(T item)
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

    public readonly Span<T> Slice(int start, int length)
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.Slice(start, length);
    }

    public static implicit operator T*(NativeRefArray<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeRefArray<T> value) => value.AsSpan();
}
