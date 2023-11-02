using System.Runtime.InteropServices;

namespace Age.Core.Unsafe;

public unsafe record struct Pointer(int Size = 1) : IDisposable
{
    private nint handler = Marshal.AllocHGlobal(Size);

    public void Dispose()
    {
        Marshal.FreeHGlobal(this.handler);
        this.handler = default;
    }

    public readonly T* As<T>() where T : unmanaged => (T*)this.handler;
    public readonly T Get<T>(int offset) where T : unmanaged => ((T*)this.handler)[offset];
    public readonly T Set<T>(int offset, T value) where T : unmanaged => ((T*)this.handler)[offset] = value;

    public static implicit operator nint(Pointer value) => value.handler;
}

public unsafe record struct Pointer<T>(int Size = 1) : IDisposable where T : unmanaged
{
    private nint handler = Marshal.AllocHGlobal(sizeof(T) * Size);

    public readonly T this[int index]
    {
        get => ((T*)this.handler)[index];
        set => ((T*)this.handler)[index] = value;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(this.handler);
        this.handler = default;
    }

    public static implicit operator T*(Pointer<T> value) => (T*)value.handler;
}
