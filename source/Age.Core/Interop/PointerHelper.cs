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

        bytes.AsSpan().CopyTo(new Span<byte>(pValue, bytes.Length));

        pValue[bytes.Length] = 0;

        return pValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullIfDefault<T>(T* pointer) where T : unmanaged =>
        (*pointer).Equals(default(T)) ? null : pointer;

    public static unsafe T* GetValuePointer<T>(T?* pointer) where T : unmanaged
    {
        var offset = Marshal.OffsetOf<T?>("value");

        return pointer->HasValue ? (T*)&((byte*)pointer)[offset] : null;
    }
}
