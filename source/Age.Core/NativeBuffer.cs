using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core;

[DebuggerTypeProxy(typeof(NativeBuffer<>.DebugView))]
public unsafe partial record struct NativeBuffer<T>(Pointer<T> Pointer, int Length) : IEquatable<NativeBuffer<T>> where T : unmanaged
{
    public readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Pointer.Value[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.Pointer.Value[index] = value;
    }

    public readonly Span<T> AsSpan() =>
        new(this.Pointer, this.Length);

    public override readonly string ToString() =>
        $"Length = {this.Length}";

    public static implicit operator T*(NativeBuffer<T> buffer) => buffer.Pointer;
    public static implicit operator Span<T>(NativeBuffer<T> buffer) => new(buffer.Pointer, buffer.Length);
}
