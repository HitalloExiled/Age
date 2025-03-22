using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe class NativeArray(int lenght = 1) : IDisposable
{
    private void* buffer = NativeMemory.Alloc((uint)lenght);
    private bool disposed;

    private int Length => lenght;

    private void CheckOffset(int index)
    {
        if (index >= lenght)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void CheckDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, this);

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

    public void* AsPointer() => this.buffer;
    public T* AsPointer<T>() where T : unmanaged => (T*)this.buffer;

    public ref T Get<T>(int offset) where T : unmanaged
    {
        this.CheckDisposed();
        this.CheckOffset(offset);

        return ref ((T*)this.buffer)[offset];
    }

    public T Set<T>(int offset, in T value) where T : unmanaged
    {
        this.CheckDisposed();
        this.CheckOffset(offset);

        return ((T*)this.buffer)[offset] = value;
    }

    public static T* ToPointer<T>(IList<T> values) where T : unmanaged
    {
        var ptr = (T*)NativeMemory.Alloc((uint)(sizeof(T) * values.Count));

        for (var i = 0; i < values.Count; i++)
        {
            ptr[i] = values[i];
        }

        return ptr;
    }

    public static implicit operator nint(NativeArray value) => new(value.buffer);
}

public unsafe class NativeArray<T>(int lenght = 1) : IDisposable where T : unmanaged
{
    private bool disposed;
    private T* buffer = (T*)NativeMemory.Alloc((uint)(sizeof(T) * lenght));

    public NativeArray(scoped ReadOnlySpan<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }
    }

    public T this[int index]
    {
        get
        {
            this.CheckDisposed();
            this.CheckIndex(index);

            return this.buffer[index];
        }
        set
        {
            this.CheckDisposed();
            this.CheckIndex(index);

            this.buffer[index] = value;
        }
    }

    ~NativeArray() => this.Dispose();

    private void CheckDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, this);

    private void CheckIndex(int index)
    {
        if (index >= lenght)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public T* AsPointer() => this.buffer;

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
}
