
using System.Runtime.CompilerServices;
using System.Text;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(byte[] source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ConvertToString() =>
            Encoding.Default.GetString(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ConvertToString(Encoding encoding) =>
            encoding.GetString(source);
    }

    extension<T>(Unsafe) where T : unmanaged, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref readonly TTo AsReadOnly<TTo>(scoped in T value) where TTo : unmanaged, allows ref struct =>
            ref Unsafe.AsRef<TTo>((TTo*)Unsafe.AsPointer(in value));
    }
}
