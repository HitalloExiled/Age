using System.Runtime.InteropServices;

namespace Age.Core.Collections;

public unsafe struct UnsafeListBuffer<T> where T : unmanaged
{
    public T* Buffer { get; private set; }

    public int Capacity
    {
        readonly get => field;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, this.Count);

            if (field == value)
			{
				return;
			}

			this.Buffer = (T*)NativeMemory.Realloc(this.Buffer, (uint)(sizeof(T) * value));

            field = value;
        }
    }

    public int Count { get; private set; }

    public T this[uint index]
    {
        readonly get => this[(int)index];
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

    public readonly bool IsEmpty => this.Count == 0;

    public readonly Span<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(this.Count);

            return this.Slice(start, length);
        }
    }

    public UnsafeListBuffer(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.Capacity = capacity;
        }
    }

    public UnsafeListBuffer(ReadOnlySpan<T> values) : this(values.Length)
    {
        this.Count = values.Length;

        values.CopyTo(this.AsSpan());
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

    public ref T Add()
    {
        this.EnsureCapacity();

        ref var element = ref this.Buffer[this.Count];

        this.Count++;

        return ref element;
    }

    public void Add(T item)
    {
        ref var element = ref this.Add();

        element = item;
    }

    public readonly Span<T> AsSpan() =>
        new(this.Buffer, this.Count);

    public void Clear() =>
        this.Count = 0;

    public readonly bool Contains(T item) =>
        this.IndexOf(item) > -1;

    public readonly void CopyTo(Span<T> array, int startIndex) =>
        this.AsSpan()[startIndex..].CopyTo(array);

    public void Dispose()
    {
        NativeMemory.Free(this.Buffer);
        this.Buffer = default;
        this.Count  = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity > this.Capacity)
        {
            this.Capacity = capacity;
        }
    }

    public readonly UnsafeEnumerator<T> GetEnumerator() =>
        new(this.Buffer, this.Count);

    public readonly int IndexOf(T item) =>
        this.AsSpan().IndexOf(item);

    public void Insert(int index, in T item)
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
            var source      = new Span<T>(this.Buffer + index, length);
            var destination = new Span<T>(this.Buffer + index + 1, length);

            source.CopyTo(destination);
        }

        this.Buffer[index] = item;
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
            var source      = new Span<T>(this.Buffer + endIndex,   length);
            var destination = new Span<T>(this.Buffer + startIndex, length);

            source.CopyTo(destination);
        }

        this.Count = int.Max(this.Count - count, 0);
    }

    public readonly Span<T> Slice(int start, int length) =>
        new(this.Buffer + start, length);
}
