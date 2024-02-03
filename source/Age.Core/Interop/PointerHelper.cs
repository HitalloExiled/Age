using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Interop;

public unsafe static class PointerHelper
{
    public static byte* Alloc(string? value)
    {
        if (value == null)
        {
            return null;
        }

        var bytes = Encoding.UTF8.GetBytes(value);

        var pValue = (byte*)NativeMemory.Alloc((uint)bytes.Length + 1);

        Copy(bytes, pValue, (uint)bytes.Length);

        pValue[bytes.Length] = 0;

        return pValue;
    }

    public static byte** Alloc(IList<string> source)
    {
        var ppData = (byte**)NativeMemory.Alloc((uint)(sizeof(byte*) * source.Count));

        for (var i = 0; i < source.Count; i++)
        {
            ppData[i] = Alloc(source[i]);
        }

        return ppData;
    }

    public static T* Alloc<T>(in T value) where T : unmanaged
    {
        var pointer = (T*)NativeMemory.Alloc((uint)sizeof(T));

        *pointer = value;

        return pointer;
    }

    public static T* Alloc<T>(T[] source) where T : unmanaged
    {
        var ptr = (T*)NativeMemory.Alloc((uint)(sizeof(T) * source.Length));

        Copy(source, ptr);

        return ptr;
    }

    public static T* Alloc<T>(Span<T> source) where T : unmanaged
    {
        var ptr = (T*)NativeMemory.Alloc((uint)(sizeof(T) * source.Length));

        Copy(source, ptr);

        return ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T* source, T[] destination, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(nint source, T[] destination, uint length) where T : unmanaged =>
        Copy((T*)source, destination, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(Span<T> source, T* destination, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(Span<T> source, nint destination, uint length) where T : unmanaged =>
        Copy(source, (T*)destination, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(Span<T> source, nint destination) where T : unmanaged =>
        Copy(source, destination, (uint)source.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(Span<T> source, T* destination) where T : unmanaged =>
        Copy(source, destination, (uint)source.Length);

    public static void Free<T>(T** pointer, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            Free(pointer[i]);
            pointer[i] = null;
        }

        NativeMemory.Free(pointer);
    }

    public static void Free<T>(T* pointer) where T : unmanaged
    {
        if (pointer != default)
        {
            NativeMemory.Free(pointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? NullIfDefault<T>(in T target) where T : unmanaged =>
        target.Equals(default(T)) ? null : target;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullIfDefault<T>(in T target, T* pointer) where T : unmanaged =>
        target.Equals(default(T)) ? null : pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(T* source, uint length) where T : unmanaged =>
        new Span<T>(source, (int)length).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(nint source, uint length) where T : unmanaged =>
        ToArray((T*)source, length);

    public static string[] ToArray(byte** ppSource, uint length)
    {
        var result = new string[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = Marshal.PtrToStringAnsi((nint)ppSource[i])!;
        }

        return result;
    }
}
