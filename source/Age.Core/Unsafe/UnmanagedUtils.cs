using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Unsafe;

public static class UnmanagedUtils
{

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(T* source, T[] destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(nint source, T[] destination, int length) where T : unmanaged =>
        Copy((T*)source, destination, length);


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(Span<T> source, T* destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(T[] source, nint destination, int length) where T : unmanaged =>
        Copy(source, (T*)destination, length);

    public static unsafe T[] PointerToArray<T>(T* source, int length) where T : unmanaged
    {
        var result = new T[length];

        Copy(source, result, length);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe T[] PointerToArray<T>(nint source, int length) where T : unmanaged =>
        PointerToArray((T*)source, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void ZeroFill(nint pointer, int size)
    {
        for (var i = 0; i < size; i++)
        {
            Marshal.WriteByte(pointer + i, 0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void ZeroFill(byte* pointer, int size) =>
        ZeroFill((nint)pointer, size);
}