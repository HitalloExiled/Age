using System.Runtime.CompilerServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static T CombineFlag<T>(this T flag, T other) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = (byte)(Unsafe.As<T, byte>(ref flag) | Unsafe.As<T, byte>(ref other));

                return Unsafe.As<byte, T>(ref byteValue);
            }
            case 2:
            {
                var shortValue = (short)(Unsafe.As<T, short>(ref flag) | Unsafe.As<T, short>(ref other));

                return Unsafe.As<short, T>(ref shortValue);
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref flag) | Unsafe.As<T, int>(ref other);

                return Unsafe.As<int, T>(ref intValue);
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref flag) | Unsafe.As<T, long>(ref other);

                return Unsafe.As<long, T>(ref longValue);
            }
            default:
                throw new NotSupportedException($"Enum with size of {sizeof(T)} are not supported");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool HasAnyFlag<T>(this T flag, T other) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = Unsafe.As<T, byte>(ref flag);
                var byteFlag  = Unsafe.As<T, byte>(ref other);

                return (byteValue & byteFlag) != 0;
            }
            case 2:
            {
                var shortValue = Unsafe.As<T, short>(ref flag);
                var shortFlag  = Unsafe.As<T, short>(ref other);

                return (shortValue & shortFlag) != 0;
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref flag);
                var intFlag  = Unsafe.As<T, int>(ref other);

                return (intValue & intFlag) != 0;
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref flag);
                var longFlag  = Unsafe.As<T, long>(ref other);

                return (longValue & longFlag) != 0;
            }
            default:
                throw new NotSupportedException($"Enum with size of {sizeof(T)} are not supported");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool HasFlags<T>(this T flag, T other) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = Unsafe.As<T, byte>(ref flag);
                var byteFlag  = Unsafe.As<T, byte>(ref other);

                return (byteValue & byteFlag) == byteFlag;
            }
            case 2:
            {
                var shortValue = Unsafe.As<T, short>(ref flag);
                var shortFlag  = Unsafe.As<T, short>(ref other);

                return (shortValue & shortFlag) == shortFlag;
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref flag);
                var intFlag  = Unsafe.As<T, int>(ref other);

                return (intValue & intFlag) == intFlag;
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref flag);
                var longFlag  = Unsafe.As<T, long>(ref other);

                return (longValue & longFlag) == longFlag;
            }
            default:
                throw new NotSupportedException("Enum with size of " + Unsafe.SizeOf<T>() + " are not supported");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSingleBit<T>(this T flag) where T : unmanaged, Enum
    {
        var value = flag.GetValue();

        return value != 0 && (value & (value - 1)) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static long GetValue<T>(this T flag) where T : unmanaged, Enum =>
        sizeof(T) switch
        {
            1 => Unsafe.As<T, byte>(ref flag),
            2 => Unsafe.As<T, short>(ref flag),
            4 => Unsafe.As<T, int>(ref flag),
            8 => Unsafe.As<T, long>(ref flag),
            _ => throw new NotSupportedException($"Enum with size of {sizeof(T)} are not supported"),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static T RemoveFlag<T>(this T flag, T other) where T : unmanaged, Enum
    {
        switch (sizeof(T))
        {
            case 1:
            {
                var byteValue = (byte)(Unsafe.As<T, byte>(ref flag) & ~Unsafe.As<T, byte>(ref other));

                return Unsafe.As<byte, T>(ref byteValue);
            }
            case 2:
            {
                var shortValue = (short)(Unsafe.As<T, short>(ref flag) & ~Unsafe.As<T, short>(ref other));

                return Unsafe.As<short, T>(ref shortValue);
            }
            case 4:
            {
                var intValue = Unsafe.As<T, int>(ref flag) & ~Unsafe.As<T, int>(ref other);

                return Unsafe.As<int, T>(ref intValue);
            }
            case 8:
            {
                var longValue = Unsafe.As<T, long>(ref flag) & ~Unsafe.As<T, long>(ref other);

                return Unsafe.As<long, T>(ref longValue);
            }
            default:
                throw new NotSupportedException($"Enum with size of {sizeof(T)} are not supported");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static T[] SplitFlags<T>(this T flags) where T : unmanaged, Enum
    {
        var maxSize = sizeof(T) * 8;
        var raw     = flags.GetValue();

        Span<T> entries = stackalloc T[maxSize];

        var count = 0;

        for (var i = 0; i < maxSize * 8; i++)
        {
            var flag = 1L << i;

            if ((raw & flag) != 0)
            {
                entries[count++] = maxSize switch
                {
                    1 => Unsafe.As<byte,  T>(ref Unsafe.As<long, byte>(ref flag)),
                    2 => Unsafe.As<short, T>(ref Unsafe.As<long, short>(ref flag)),
                    4 => Unsafe.As<int,   T>(ref Unsafe.As<long, int>(ref flag)),
                    8 => Unsafe.As<long,  T>(ref flag),
                    _ => throw new NotSupportedException($"Enum with size of {maxSize} is not supported")
                };
            }
        }

        return [..entries[..count]];
    }
}
