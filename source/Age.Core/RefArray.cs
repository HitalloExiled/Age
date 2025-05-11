using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Age.Core;

[DebuggerTypeProxy(typeof(RefArray<>.DebugView))]
public unsafe ref partial struct RefArray<T>(int length = 0) : IDisposable, IEnumerable<T> where T : unmanaged
{
    private T* buffer = (T*)NativeMemory.AllocZeroed((uint)(sizeof(T) * length));

    public int Length { get; private set; } = length;

    public readonly T this[uint index]
    {
        get => this[(int)index];
        set => this[(int)index] = value;
    }

    public readonly bool IsEmpty => this.Length == 0;

    public readonly T this[int index]
    {
        get
        {
            this.CheckIndex(index);

            return this.buffer[index];
        }
        set
        {
            this.CheckIndex(index);

            this.buffer[index] = value;
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

    public RefArray(uint length) : this((int)length)
    { }

    public RefArray(scoped ReadOnlySpan<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }
    }

    private readonly void CheckIndex(int index)
    {
        if (index >= this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Dispose()
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
        this.Length = 0;
    }

    readonly IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        this.GetEnumerator();

    public readonly T* AsPointer() =>
        this.buffer;

    public readonly Span<T> AsSpan() =>
        new(this.buffer, this.Length);

    public readonly void Clear() =>
        this.AsSpan().Clear();

    public bool Contains(T item) =>
        this.IndexOf(item) > -1;

    public readonly void CopyTo(T[] array, int arrayIndex) =>
        this.AsSpan()[arrayIndex..].CopyTo(array);

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        new(this.buffer, this.Length);

    public int IndexOf(T item)
    {
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

    public readonly Span<T> Slice(int start, int length) =>
        new(this.buffer + start, length);

    public static implicit operator Span<T>(RefArray<T> value) => value.AsSpan();
}
