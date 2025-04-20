using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Age.Core;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(RefList<>.DebugView))]
public unsafe ref partial struct RefList<T> : IEnumerable<T>, IDisposable where T : unmanaged
{
    private T* buffer;

    public int Capacity
    {
        readonly get => field;
        set
        {
            if (value == 0)
            {
                NativeMemory.Free(this.buffer);

                this.Count = 0;
            }
            else
            {
                this.buffer = (T*)NativeMemory.Realloc(this.buffer, (uint)(sizeof(T) * value));

                if (value > field)
                {
                    for (var i = field; i < value; i++)
                    {
                        this.buffer[i] = default;
                    }
                }

                if (value < this.Count)
                {
                    this.Count = value;
                }
            }

            field = value;
        }
    }

    public int Count { get; private set; }

    public T this[uint index]
    {
        get => this[(int)index];
        set => this[(int)index] = value;
    }

    public T this[int index]
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

    public readonly RefList<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(this.Count);

            return this.Slice(start, length);
        }
    }

    public RefList(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.buffer = (T*)NativeMemory.AllocZeroed((uint)(sizeof(T) * capacity));

            this.Capacity = capacity;
        }
    }

    public RefList(scoped ReadOnlySpan<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }

        this.Count = values.Length;
    }

    private readonly void CheckIndex(int index)
    {
        if (index >= this.Count)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void EnsureCapacity()
    {
        if (this.Count + 1 > this.Capacity)
        {
            this.Capacity = int.Min(this.Capacity == 0 ? 4 : this.Count * 2, int.MaxValue);
        }
    }

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        this.GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    public ref T Add()
    {
        this.EnsureCapacity();

        ref var element = ref this.buffer[this.Count];

        this.Count++;

        return ref element;
    }

    public void Add(T item)
    {
        ref var element = ref this.Add();

        element = item;
    }

    public readonly T* AsPointer() =>
        this.buffer;

    public readonly Span<T> AsSpan() =>
        new(this.buffer, this.Count);

    public void Clear() =>
        this.Count = 0;

    public bool Contains(T item) =>
        this.IndexOf(item) > -1;

    public readonly void CopyTo(T[] array, int arrayIndex) =>
        this.AsSpan()[arrayIndex..].CopyTo(array);

    public void EnsureCapacity(int capacity)
    {
        if (capacity > this.Capacity)
        {
            this.Capacity = capacity;
        }
    }

    public void Dispose()
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
        this.Count  = 0;
    }

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        new(this.buffer, this.Count);

    public int IndexOf(T item)
    {
        for (var i = 0; i < this.Count; i++)
        {
            if (this.buffer[i].Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index > this.Count)
        {
            throw new IndexOutOfRangeException();
        }

        this.EnsureCapacity();

        this.Count++;

        var length = this.Count - index - 1;

        if (length > 0)
        {
            var source      = new Span<T>(this.buffer + index, length);
            var destination = new Span<T>(this.buffer + index + 1, length);

            source.CopyTo(destination);
        }

        this.buffer[index] = item;
    }

    public bool Remove(T item)
    {
        var index = this.IndexOf(item);

        if (index > -1)
        {
            this.RemoveAt(index);

            return true;
        }

        return false;
    }

    public void RemoveAt(int index) =>
        this.RemoveAt(index, 1);

    public void RemoveAt(int startIndex, int count)
    {
        this.CheckIndex(startIndex);

        var endIndex = startIndex + count;
        var length   = this.Count - endIndex;

        if (length > 0)
        {
            var source      = new Span<T>(this.buffer + endIndex,   length);
            var destination = new Span<T>(this.buffer + startIndex, length);

            source.CopyTo(destination);
        }

        this.Count = int.Max(this.Count - count, 0);
    }

    public readonly RefList<T> Slice(int start, int length) =>
        new(new Span<T>(this.buffer + start, length));

    public static implicit operator Span<T>(in RefList<T> value) => value.AsSpan();
}
