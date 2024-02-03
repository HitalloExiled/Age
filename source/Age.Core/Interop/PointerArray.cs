using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe record struct PointerArray(int Lenght = 1) : IDisposable
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

    public static implicit operator nint(PointerArray value) => value.handler;
}

public unsafe record struct PointerArray<T>(int Lenght = 1) : IDisposable where T : unmanaged
{
    private nint handler = (nint)NativeMemory.Alloc((uint)(sizeof(T) * Lenght));

    public PointerArray(IList<T> values) : this(values.Count)
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

    public static implicit operator T*(PointerArray<T> value) => (T*)value.handler;
}
