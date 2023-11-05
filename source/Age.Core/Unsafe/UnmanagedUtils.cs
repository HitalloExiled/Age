using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Unsafe;

public unsafe static class UnmanagedUtils
{

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Copy<T>(T* source, T[] destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Copy<T>(nint source, T[] destination, int length) where T : unmanaged =>
        Copy((T*)source, destination, length);


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Copy<T>(Span<T> source, T* destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Copy<T>(T[] source, nint destination, int length) where T : unmanaged =>
        Copy(source, (T*)destination, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T? NullIfDefault<T>(in T target) where T : unmanaged =>
        target.Equals(default(T)) ? null : target;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T* NullIfDefault<T>(in T target, T* pointer) where T : unmanaged =>
        target.Equals(default(T)) ? null : pointer;

    public static T[] PointerToArray<T>(T* source, int length) where T : unmanaged
    {
        var result = new T[length];

        Copy(source, result, length);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] PointerToArray<T>(nint source, int length) where T : unmanaged =>
        PointerToArray((T*)source, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ZeroFill(nint pointer, int size)
    {
        for (var i = 0; i < size; i++)
        {
            Marshal.WriteByte(pointer + i, 0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ZeroFill(byte* pointer, int size) =>
        ZeroFill((nint)pointer, size);
}
