using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool HasFlags<T>(this T value, T flags) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = Unsafe.As<T, byte>(ref value);
                var byteFlag  = Unsafe.As<T, byte>(ref flags);

                return (byteValue & byteFlag) == byteFlag;
            }
            case 2:
            {
                var shortValue = Unsafe.As<T, short>(ref value);
                var shortFlag  = Unsafe.As<T, short>(ref flags);

                return (shortValue & shortFlag) == shortFlag;
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref value);
                var intFlag  = Unsafe.As<T, int>(ref flags);

                return (intValue & intFlag) == intFlag;
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref value);
                var longFlag  = Unsafe.As<T, long>(ref flags);

                return (longValue & longFlag) == longFlag;
            }
            default:
                throw new NotSupportedException("Enum with size of " + Unsafe.SizeOf<T>() + " are not supported");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool HasAnyFlag<T>(this T value, T flags) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = Unsafe.As<T, byte>(ref value);
                var byteFlag  = Unsafe.As<T, byte>(ref flags);

                return (byteValue & byteFlag) != 0;
            }
            case 2:
            {
                var shortValue = Unsafe.As<T, short>(ref value);
                var shortFlag  = Unsafe.As<T, short>(ref flags);

                return (shortValue & shortFlag) != 0;
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref value);
                var intFlag  = Unsafe.As<T, int>(ref flags);

                return (intValue & intFlag) != 0;
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref value);
                var longFlag  = Unsafe.As<T, long>(ref flags);

                return (longValue & longFlag) != 0;
            }
            default:
                throw new NotSupportedException($"Enum with size of {sizeof(T)} are not supported");
        }
    }
}
