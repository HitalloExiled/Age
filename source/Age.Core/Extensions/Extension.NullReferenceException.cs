using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(NullReferenceException)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateNotNull<T>(T? value, string? message = null)
        {
            if (value == null)
            {
                throw new NullReferenceException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ValidateNotNull<T>(T* value, string? message = null) where T : unmanaged, allows ref struct
        {
            if (value == null)
            {
                throw new NullReferenceException(message);
            }
        }
    }
}
