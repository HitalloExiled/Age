using System.Runtime.InteropServices;

namespace Age.Core.Collections;

public unsafe struct UnsafeArrayBuffer<T>(int size) where T : unmanaged
{
    public T* Buffer { get; private set; } = (T*)NativeMemory.AllocZeroed((uint)(sizeof(T) * size));

    public int Length { get; private set; } = size;

    public readonly T this[uint index]
    {
        get => this[(int)index];
        set => this[(int)index] = value;
    }

    public readonly T this[int index]
    {
        get
        {
            this.CheckIndex(index);

            return this.Buffer[index];
        }
        set
        {
            this.CheckIndex(index);

            this.Buffer[index] = value;
        }
    }

    public readonly Span<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(this.Length);

            return this.Slice(start, length);
        }
    }

    public readonly bool IsEmpty => this.Length == 0;

    public UnsafeArrayBuffer(uint length) : this((int)length)
    { }

    public UnsafeArrayBuffer(ReadOnlySpan<T> values) : this(values.Length) =>
        values.CopyTo(this.AsSpan());

    private readonly void CheckIndex(int index)
    {
        if (index >= this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Dispose()
    {
        NativeMemory.Free(this.Buffer);

        this.Buffer = default;
        this.Length = 0;
    }

    public readonly Span<T> AsSpan() =>
        new(this.Buffer, this.Length);

    public readonly void Clear() =>
        this.AsSpan().Clear();

    public readonly bool Contains(T item) =>
        this.AsSpan().Contains(item);

    public readonly void CopyTo(Span<T> span) =>
        this.AsSpan().CopyTo(span);

    public readonly void CopyTo(Span<T> array, int startIndex) =>
        this.AsSpan()[startIndex..].CopyTo(array);

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        new(this.Buffer, this.Length);

    public readonly int IndexOf(T item) =>
        this.AsSpan().IndexOf(item);

    public void Resize(int size)
    {
        this.Buffer = (T*)NativeMemory.Realloc(this.Buffer, (uint)(sizeof(T) * size));

        if (size > this.Length)
        {
            new Span<T>(this.Buffer, size)[this.Length..].Clear();
        }

        this.Length = size;
    }

    public void ResizeCopy(ReadOnlySpan<T> source)
    {
        if (source.Length != this.Length)
        {
            this.Resize(source.Length);
        }

        source.CopyTo(this.AsSpan());
    }

    public readonly Span<T> Slice(int start, int length) =>
        new(this.Buffer + start, length);
}
