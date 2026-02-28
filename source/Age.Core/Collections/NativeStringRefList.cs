using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringRefList))]
public ref partial struct NativeStringRefList
{
    private bool disposed;

    private UnsafeStringListBuffer unsafeBuffer;

    public readonly string this[int index]
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

    public int Capacity
    {
        readonly get => this.unsafeBuffer.Capacity;
        set
        {
            this.ThrowIfDisposed();

            this.unsafeBuffer.Capacity = value;
        }
    }

    public readonly unsafe byte** Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public readonly int  Count   => this.unsafeBuffer.Count;
    public readonly bool IsEmpty => this.unsafeBuffer.IsEmpty;

    public NativeStringRefList(int capacity = 0) =>
        this.unsafeBuffer = new(capacity);

    public NativeStringRefList(ReadOnlySpan<string?> values) =>
        this.unsafeBuffer = new(values);

    private readonly void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, typeof(NativeStringRefList));

    public void Add(string? value)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Add(value);
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Clear();
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

    public readonly Span<string>.Enumerator GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public void Remove(int startIndex, int count = 1)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Remove(startIndex, count);
    }

    public readonly string[] ToArray()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.ToArray();
    }
}
