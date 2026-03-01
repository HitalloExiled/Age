using System.Runtime.InteropServices;

namespace Age.Core;

public unsafe ref struct UnmanagedArray<T>(T* buffer, int length) where T : unmanaged
{
    public readonly T* Buffer = buffer;
    public readonly int Length => length;

    public readonly void Dispose() =>
        NativeMemory.Free(this.Buffer);

    public readonly Span<T> AsSpan() =>
        new(this.Buffer, this.Length);

    public static implicit operator Span<T>(UnmanagedArray<T> unmanagedArray) => unmanagedArray.AsSpan();
}
