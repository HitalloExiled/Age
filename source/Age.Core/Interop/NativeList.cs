using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe class NativeList<T> : IDisposable where T : unmanaged
{
    private int  capacity;
    private T*   buffer;
    private bool disposed;

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

    public NativeList(Span<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }

        this.Count = values.Length;
    }

    ~NativeList() => this.Dispose();

    public ref T this[int index]
    {
        get
        {
            this.CheckDisposed();
            this.CheckIndex(index);

            return ref this.buffer[index];
        }
    }

    private void CheckDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, this);

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

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            NativeMemory.Free(this.buffer);
            this.buffer = default;
        }

        GC.SuppressFinalize(this);
    }

    public ref T Add()
    {
        this.CheckDisposed();
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
        this.CheckDisposed();

        for (var i = 0; i < this.Count; i++)
        {
            this.buffer[i] = default;
        }

        this.Count = 0;
    }

    public void Remove(int index)
    {
        this.CheckIndex(index);
        this.CheckDisposed();

        this.Count -= 1;

        for (var i = index; i < this.Count; i++)
        {
            this.buffer[i] = this.buffer[i + 1];
        }

        this.buffer[this.Count] = default;
    }

    public T* AsPointer() => this.buffer;
    public Span<T> AsSpan() => new(this.buffer, this.Count);
}
