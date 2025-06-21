using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static void CopyTo(scoped ReadOnlySpan<byte> source, int sizeOfSource, scoped Span<byte> destination, int sizeOfDestination, int length, bool alignToEnd)
    {
        if (sizeOfSource == sizeOfDestination)
        {
            source.CopyTo(destination);
            return;
        }

        ref var sourceRef = ref MemoryMarshal.GetReference(source);

        if (sizeOfSource < sizeOfDestination)
        {
            var offset = alignToEnd ? sizeOfDestination - sizeOfSource : 0;

            for (var i = 0; i < length; i++)
            {
                var sourceSlice      = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref sourceRef, i * sizeOfSource), sizeOfSource);
                var destinationSlice = MemoryMarshal.CreateSpan(ref destination[(i * sizeOfDestination) + offset], sizeOfDestination);

                sourceSlice.CopyTo(destinationSlice);
            }
        }
        else
        {
            var offset = alignToEnd ? sizeOfSource - sizeOfDestination : 0;

            for (var i = 0; i < length; i++)
            {
                var sourceSlice      = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref sourceRef, (i * sizeOfSource) + offset), sizeOfDestination);
                var destinationSlice = MemoryMarshal.CreateSpan(ref destination[i * sizeOfDestination], sizeOfDestination);

                sourceSlice.CopyTo(destinationSlice);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<U> Cast<T, U>(this Span<T> span)
    where T : unmanaged
    where U : unmanaged =>
        MemoryMarshal.Cast<T, U>(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyTo<TSource, TDestination>(this scoped ReadOnlySpan<TSource> source, scoped Span<TDestination> destination, bool alignToEnd = false)
    where TSource : unmanaged
    where TDestination : unmanaged
    {
        if (source.Length > destination.Length)
        {
            throw new InvalidOperationException($"{nameof(destination)} length must be greater or equal than {nameof(source)} length");
        }

        CopyTo(source.Cast<TSource, byte>(), sizeof(TSource), destination.Cast<TDestination, byte>(), sizeof(TDestination), source.Length, alignToEnd);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyTo<TSource>(this scoped ReadOnlySpan<TSource> source, scoped Span<byte> destination, int sizeOfDestination, bool alignToEnd = false)
    where TSource : unmanaged
    {
        var destinationLength = destination.Length / sizeOfDestination;

        if (destinationLength < source.Length)
        {
            throw new InvalidOperationException($"{nameof(destination)}.Length / {sizeOfDestination} must be greater or equal than {nameof(source)}.Length");
        }

        CopyTo(source.Cast<TSource, byte>(), sizeof(TSource), destination, sizeOfDestination, source.Length, alignToEnd);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyTo<TDestination>(this scoped ReadOnlySpan<byte> source, int sizeOfSource, scoped Span<TDestination> destination, bool alignToEnd = false)
    where TDestination : unmanaged
    {
        var sourceLength = source.Length / sizeOfSource;

        if (destination.Length < sourceLength)
        {
            throw new InvalidOperationException($"{nameof(destination)}.Length must be greater or equal than {nameof(source)}.Length / {sizeOfSource}");
        }

        CopyTo(source, sizeOfSource, destination.Cast<TDestination, byte>(), sizeof(TDestination), sourceLength, alignToEnd);
    }
}
