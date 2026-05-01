
using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Unsafe)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref readonly TTo AsReadOnly<TFrom, TTo>(in TFrom value)
        where TFrom : unmanaged, allows ref struct
        where TTo : unmanaged, allows ref struct =>
            ref Unsafe.AsRef<TTo>((TTo*)Unsafe.AsPointer(in value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal<T>(in T? left, in T? right) where T : unmanaged
        {
            if (left.HasValue != right.HasValue)
            {
                return false;
            }

            var leftSpan  = new ReadOnlySpan<T>(in Nullable.GetValueRefOrDefaultRef(in left)).Cast<T, byte>();
            var rightSpan = new ReadOnlySpan<T>(in Nullable.GetValueRefOrDefaultRef(in right)).Cast<T, byte>();

            return leftSpan.SequenceEqual(rightSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool BitEqual<T>(in T left, in T right) where T : unmanaged, allows ref struct
        {
            var leftSpan  = new ReadOnlySpan<byte>(Unsafe.AsPointer(in left),  sizeof(T));
            var rightSpan = new ReadOnlySpan<byte>(Unsafe.AsPointer(in right), sizeof(T));

            return leftSpan.SequenceEqual(rightSpan);
        }
    }
}
