
using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Unsafe)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref readonly TTo AsReadOnly<TFrom, TTo>(scoped in TFrom value)
        where TFrom : unmanaged, allows ref struct
        where TTo : unmanaged, allows ref struct =>
            ref Unsafe.AsRef<TTo>((TTo*)Unsafe.AsPointer(in value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal<T>(in T? left, in T? right) where T : unmanaged
        {
            var leftSpan  = new ReadOnlySpan<T>(in Nullable.GetValueRefOrDefaultRef(in left));
            var rightSpan = new ReadOnlySpan<T>(in Nullable.GetValueRefOrDefaultRef(in right));

            return leftSpan.SequenceEqual(rightSpan);
        }
    }
}
