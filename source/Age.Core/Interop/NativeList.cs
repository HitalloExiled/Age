using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe class NativeList<T> : Disposable, IEnumerable<T> where T : unmanaged
{
    private int  capacity;
    private T*   buffer;

    public int Capacity
    {
        get => this.capacity;
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

                if (value > this.capacity)
                {
                    for (var i = this.capacity; i < value; i++)
                    {
                        this.buffer[i] = default;
                    }
                }

                if (value < this.Count)
                {
                    this.Count = value;
                }
            }

            this.capacity = value;
        }
    }

    public int Count { get; private set; }

    public NativeList(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.buffer = (T*)NativeMemory.AllocZeroed((uint)(sizeof(T) * capacity));

            this.capacity = capacity;
        }
    }

    public NativeList(scoped ReadOnlySpan<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }

        this.Count = values.Length;
    }

    public ref T this[int index]
    {
        get
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            return ref this.buffer[index];
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

    protected override void Disposed(bool disposed)
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
    }

    public ref T Add()
    {
        this.ThrowIfDisposed();
        this.EnsureCapacity();

        ref var element = ref this.buffer[this.Count];

        this.Count++;

        return ref element;
    }

    public void Add(in T item)
    {
        ref var element = ref this.Add();

        element = item;
    }

    public void Clear()
    {
        this.ThrowIfDisposed();
        this.Count = 0;
    }

    public void Remove(int startIndex, int lenght = 1)
    {
        this.ThrowIfDisposed();
        this.CheckIndex(startIndex);

        var endIndex = startIndex + lenght;

        if (endIndex < this.Count)
        {
            var source      = new Span<T>(this.buffer + endIndex, this.Count - endIndex);
            var destination = new Span<T>(this.buffer + startIndex, endIndex - startIndex);

            source.CopyTo(destination);
        }

        this.Count = int.Max(this.Count - lenght, 0);
    }

    public T* AsPointer() => this.buffer;
    public Span<T> AsSpan() => new(this.buffer, this.Count);

    public IEnumerator<T> GetEnumerator()
    {
        var span = this.AsSpan().ToArray();

        for (var i = 0; i < this.Count; i++)
        {
            yield return span[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
