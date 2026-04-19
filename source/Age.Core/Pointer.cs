using System.Runtime.CompilerServices;

namespace Age.Core;

public unsafe struct Pointer<T>(T* value) where T : unmanaged
{
    public T* Value = value;

    public static implicit operator T*(Pointer<T> ptr) => ptr.Value;
    public static implicit operator Pointer<T>(T* ptr) => new(ptr);
}

public unsafe static class PointerHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullIfDefault<T>(T* pointer) where T : unmanaged =>
        (*pointer).Equals(default(T)) ? null : pointer;
}
