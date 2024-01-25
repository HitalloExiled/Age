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

    public static T* Alloc<T>(IList<T> source) where T : unmanaged
    {
        var pData = (T*)NativeMemory.Alloc((uint)(sizeof(T) * source.Count));

        for (var i = 0; i < source.Count; i++)
        {
            pData[i] = source[i];
        }

        return pData;
    }

    public static void Copy<T>(T* source, T[] destination, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    public static void Copy<T>(nint source, T[] destination, uint length) where T : unmanaged =>
        Copy((T*)source, destination, length);

    public static void Copy<T>(Span<T> source, T* destination, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    public static void Copy<T>(T[] source, nint destination, uint length) where T : unmanaged =>
        Copy(source, (T*)destination, length);

    public static void Copy<T>(T[] source, nint destination) where T : unmanaged =>
        Copy(source, destination, (uint)source.Length);

    public static void Copy<T>(T[] source, T* destination) where T : unmanaged =>
        Copy(source, destination, (uint)source.Length);

    public static void Free<T>(T* pointer) where T : unmanaged =>
        Free((nint)pointer);

    public static void Free<T>(T** pointer, uint length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            Free(pointer[i]);
            pointer[i] = null;
        }

        Free((nint)pointer);
    }

    public static T? NullIfDefault<T>(in T target) where T : unmanaged =>
        target.Equals(default(T)) ? null : target;

    public static T* NullIfDefault<T>(in T target, T* pointer) where T : unmanaged =>
        target.Equals(default(T)) ? null : pointer;



    public static T[] ToArray<T>(T* source, uint length) where T : unmanaged
    {
        var result = new T[length];

        Copy(source, result, length);

        return result;
    }

    public static void Free(nint pointer)
    {
        if (pointer != default)
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

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

    public static void ZeroFill(nint pointer, int size)
    {
        for (var i = 0; i < size; i++)
        {
            Marshal.WriteByte(pointer + i, 0);
        }
    }

    public static void ZeroFill(byte* pointer, int size) =>
        ZeroFill((nint)pointer, size);
}
