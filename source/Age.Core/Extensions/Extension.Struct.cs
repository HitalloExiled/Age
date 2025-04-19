using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    public static Span<byte> AsByteSpan<T>(this ref T value) where T : unmanaged =>
        MemoryMarshal.CreateSpan(ref value, 1).Cast<T, byte>();

    public static ref T AsStructRef<T>(this Span<byte> buffer) where T : unmanaged =>
        ref Unsafe.As<byte, T>(ref buffer[0]);
}
