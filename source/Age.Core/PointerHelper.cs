using System.Runtime.CompilerServices;

namespace Age.Core;

public unsafe static class PointerHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullIfDefault<T>(T* pointer) where T : unmanaged =>
        (*pointer).Equals(default(T)) ? null : pointer;
}
