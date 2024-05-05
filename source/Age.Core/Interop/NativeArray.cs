using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe record struct NativeArray(int Lenght = 1) : IDisposable
{
    private nint handler = (nint)NativeMemory.Alloc((uint)Lenght);

    public void Dispose()
    {
        NativeMemory.Free((void*)this.handler);
        this.handler = default;
    }

    public readonly T* As<T>() where T : unmanaged => (T*)this.handler;
    public readonly T Get<T>(int offset) where T : unmanaged => ((T*)this.handler)[offset];
    public readonly T Set<T>(int offset, T value) where T : unmanaged => ((T*)this.handler)[offset] = value;

    public static T* ToPointer<T>(IList<T> values) where T : unmanaged
    {
        var ptr = (T*)NativeMemory.Alloc((uint)(sizeof(T) * values.Count));

        for (var i = 0; i < values.Count; i++)
        {
            ptr[i] = values[i];
        }

        return ptr;
    }

    public static implicit operator nint(NativeArray value) => value.handler;
}

public unsafe record struct NativeArray<T>(int Lenght = 1) : IDisposable where T : unmanaged
{
    private nint handler = (nint)NativeMemory.Alloc((uint)(sizeof(T) * Lenght));

    public NativeArray(IList<T> values) : this(values.Count)
    {
        for (var i = 0; i < values.Count; i++)
        {
            ((T*)this.handler)[i] = values[i];
        }
    }

    public readonly T this[int index]
    {
        get => ((T*)this.handler)[index];
        set => ((T*)this.handler)[index] = value;
    }

    public void Dispose()
    {
        NativeMemory.Free((void*)this.handler);

        this.handler = default;
    }

    public static implicit operator T*(NativeArray<T> value) => (T*)value.handler;
}
