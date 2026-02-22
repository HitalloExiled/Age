using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension<T>(ref T value) where T : unmanaged
    {
        public ReadOnlySpan<byte> AsReadOnlySpan() =>
            new ReadOnlySpan<T>(in value).Cast<T, byte>();
    }

    extension(Span<byte> source)
    {
        public ref T AsStructRef<T>() where T : unmanaged =>
            ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(source));
    }

    extension(ReadOnlySpan<byte> source)
    {
        public T Read<T>() where T : unmanaged =>
            MemoryMarshal.Read<T>(source);
    }
}
