using System.Runtime.CompilerServices;

namespace Age.Core;

public unsafe partial record struct PointerArray<T>(Pointer<T> Buffer, int Length) : IEquatable<PointerArray<T>> where T : unmanaged, allows ref struct
{
    public readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Buffer.Value[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.Buffer.Value[index] = value;
    }
}
