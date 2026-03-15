using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DisposableRentedArray<>.DebugView))]
public ref partial struct DisposableRentedArray<T>(int size) where T : IDisposable
{
    private readonly T[] source = ArrayPool<T>.Shared.Rent(size);

    public readonly T this[int index]
    {
        get
        {
            this.CheckIndex(index);

            return this.source[index];
        }
        set
        {
            this.CheckIndex(index);

            this.source[index] = value;
        }
    }

    public readonly ReadOnlySpan<T> Values => this.source.AsSpan(0, size);

    public readonly int Length => size;

    private readonly void CheckIndex(int index)
    {
        if (index < 0 || index >= size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public readonly void Dispose()
    {
        foreach (var item in this.Values)
        {
            item.Dispose();
        }

        ArrayPool<T>.Shared.Return(this.source, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    }

    public readonly ReadOnlySpan<T>.Enumerator GetEnumerator() =>
        this.Values.GetEnumerator();

    public static implicit operator ReadOnlySpan<T>(DisposableRentedArray<T> disposibleSequence) => disposibleSequence.Values;
}
