using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    public static Span<byte> AsByteSpan<T>(this ref T value) where T : unmanaged =>
        new Span<T>(ref value).Cast<T, byte>();

    public static ref T AsStructRef<T>(this Span<byte> source) where T : unmanaged =>
        ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(source));

    public static T Read<T>(this ReadOnlySpan<byte> source) where T : unmanaged =>
        MemoryMarshal.Read<T>(source);
}
