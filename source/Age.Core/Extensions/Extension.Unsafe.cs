
using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension<T>(Unsafe) where T : unmanaged, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref readonly TTo AsReadOnly<TTo>(scoped in T value) where TTo : unmanaged, allows ref struct =>
            ref Unsafe.AsRef<TTo>((TTo*)Unsafe.AsPointer(in value));
    }
}
