using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringRefArray))]
public unsafe ref partial struct NativeStringRefArray : IDisposable
{
    private bool disposed;

    private UnsafeStringArrayBuffer unsafeBuffer;

    public readonly string? this[int index]
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

    public readonly byte** Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public readonly bool IsEmpty => this.unsafeBuffer.IsEmpty;
    public readonly int  Length  => this.unsafeBuffer.Length;

    public NativeStringRefArray(int size) =>
        this.unsafeBuffer = new(size);

    public NativeStringRefArray(ReadOnlySpan<string> values) =>
        this.unsafeBuffer = new(values);

    private readonly void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, typeof(NativeStringRefArray));

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.unsafeBuffer.Dispose();

        this.disposed = true;
    }

    public readonly Span<string>.Enumerator GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public readonly string[] ToArray()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.ToArray();
    }

    public static implicit operator byte**(NativeStringRefArray value) => value.Buffer;
}
