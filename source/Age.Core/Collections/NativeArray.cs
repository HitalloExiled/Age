using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Age.Core.Collections;

[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(NativeArray<>.DebugView))]
public unsafe partial class NativeArray<T>(int length = 0) : Disposable, IEnumerable<T> where T : unmanaged
{
    public static NativeArray<T> Empty { get; } = [];

    private T* buffer = (T*)NativeMemory.AllocZeroed((uint)(sizeof(T) * length));

    public int Length { get; private set; } = length;

    public T this[uint index]
    {
        get => this[(int)index];
        set => this[(int)index] = value;
    }

    public T this[int index]
    {
        get
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            return this.buffer[index];
        }
        set
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            this.buffer[index] = value;
        }
    }

    public NativeArray<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(this.Length);

            return this.Slice(start, length);
        }
    }

    public NativeArray(uint length) : this((int)length)
    { }

    public NativeArray(scoped ReadOnlySpan<T> values) : this(values.Length) =>
        values.CopyTo(this);

    private void CheckIndex(int index)
    {
        if (index >= this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
        this.Length = 0;
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        this.GetEnumerator();

    public T* AsPointer()
    {
        this.ThrowIfDisposed();

        return this.buffer;
    }

    public Span<T> AsSpan()
    {
        this.ThrowIfDisposed();

        return new(this.buffer, this.Length);
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.AsSpan().Clear();
    }

    public bool Contains(T item)
    {
        this.ThrowIfDisposed();

        return this.IndexOf(item) > -1;
    }

    public void CopyTo(Span<T> span)
    {
        this.ThrowIfDisposed();

        this.AsSpan().CopyTo(span);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        this.ThrowIfDisposed();

        this.AsSpan()[arrayIndex..].CopyTo(array);
    }

    public UnsafeEnumerator<T> GetEnumerator()
    {
        this.ThrowIfDisposed();

        return new(this.buffer, this.Length);
    }

    public int IndexOf(T item)
    {
        this.ThrowIfDisposed();

        for (var i = 0; i < this.Length; i++)
        {
            if (this.buffer[i].Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Resize(int length)
    {
        this.ThrowIfDisposed();

        this.buffer = (T*)NativeMemory.Realloc(this.buffer, (uint)(sizeof(T) * length));
        this.Length = length;
    }

    public void ResizeCopy(scoped ReadOnlySpan<T> source)
    {
        if (source.Length != this.Length)
        {
            this.Resize(source.Length);
        }

        source.CopyTo(this);
    }

    public NativeArray<T> Slice(int start, int length) =>
        new(new Span<T>(this.buffer + start, length));

    public static implicit operator Span<T>(NativeArray<T> value) => value.AsSpan();
}
