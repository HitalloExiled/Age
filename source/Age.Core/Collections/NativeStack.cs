using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Core.Collections;

public unsafe partial class NativeStack<T> : Disposable, IEnumerable<T> where T : unmanaged
{
    private T* buffer;

    public int Capacity
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            if (value == 0)
            {
                NativeMemory.Free(this.buffer);

                this.Count = 0;
            }
            else
            {
                this.buffer = (T*)NativeMemory.Realloc(this.buffer, (uint)(sizeof(T) * value));

                if (value < this.Count)
                {
                    this.Count = value;
                }
            }

            field = value;
        }
    }

    public int Count { get; private set; }

    public NativeStack(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.buffer = (T*)NativeMemory.Alloc((uint)(sizeof(T) * capacity));

            this.Capacity = capacity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        this.GetEnumerator();

    private void ThrowIfEmpty()
    {
        if (this.Count == 0)
        {
            throw new InvalidOperationException("Empty Stack");
        }
    }

    private void CheckIndex(int index)
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
            this.Capacity = int.Min(this.Capacity == 0 ? 4 : this.Capacity * 2, int.MaxValue);
        }
    }

    protected override void OnDisposed(bool disposed)
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
    }

    public ref T Peek()
    {
        this.ThrowIfEmpty();

        return ref this.buffer[this.Count - 1];
    }

    public ref T Push()
    {
        this.ThrowIfDisposed();
        this.EnsureCapacity();

        ref var element = ref this.buffer[this.Count];

        element = default;

        this.Count++;

        return ref element;
    }

    public void Push(in T item)
    {
        ref var element = ref this.Push();

        element = item;
    }

    public void Clear()
    {
        this.ThrowIfDisposed();
        this.Count = 0;
    }

    public T Pop()
    {
        this.ThrowIfDisposed();
        this.ThrowIfEmpty();

        this.Count--;

        return this.buffer[this.Count];
    }

    public Span<T> AsSpan() => new(this.buffer, this.Count);

    public Enumerator GetEnumerator() => new(this);
}
