using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeHashSet<>.DebugView))]
public unsafe partial struct NativeHashSet<T>(int capacity, bool fixedSize) : IDisposable
where T : unmanaged, IEquatable<T>
{
    private UnsafeHashSet* inner = UnsafeHashSet.Allocate<T>(capacity, fixedSize);

    public readonly int  Capacity    => UnsafeHashSet.GetCapacity(this.inner);
    public readonly int  Count       => UnsafeHashSet.GetCount(this.inner);
    public readonly bool IsCreated   => this.inner != null;
    public readonly bool IsDisposed  => this.inner == null;
    public readonly bool IsFixedSize => UnsafeHashSet.IsFixedSize(this.inner);

    public NativeHashSet() : this(0, false)
    { }

    public NativeHashSet(int capacity) : this(capacity, false)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Add(T item) =>
        UnsafeHashSet.Add(this.inner, item);

    public readonly void Clear() =>
        UnsafeHashSet.Clear(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(T item) =>
        UnsafeHashSet.Contains(this.inner, item);

    public readonly void CopyTo(Span<T> destination, int destinationIndex) =>
        UnsafeHashSet.CopyTo<T>(this.inner, Unsafe.AsPointer(ref destination[destinationIndex]), 0);

    public void Dispose()
    {
        UnsafeHashSet.Free(this.inner);
        this.inner = null;
    }

    public readonly UnsafeHashSet.Enumerator<T> GetEnumerator() =>
        UnsafeHashSet.GetEnumerator<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Remove(T item) =>
        UnsafeHashSet.Remove(this.inner, item);

    public readonly NativeArray<T> ToNativeArray()
    {
        if (this.Count == 0)
        {
            return default;
        }

        var arr = new NativeArray<T>(this.Count);

        UnsafeHashSet.CopyTo<T>(this.inner, arr.Buffer, 0);

        return arr;
    }

    public override readonly string ToString() =>
        this.IsCreated ? $"Count = {this.Count}" : "";
}
