using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeList<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeList))]
public unsafe partial struct NativeList<T>(int capacity, bool fixedSize = false) : IDisposable where T : unmanaged
{
    private UnsafeList* inner = UnsafeList.Allocate<T>(capacity, fixedSize);

    public readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeList.Get<T>(this.inner, index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => UnsafeList.Set(this.inner, index, value);
    }

    public readonly T* Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (T*)UnsafeList.GetBuffer(this.inner);
    }

    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => UnsafeList.SetCapacity(this.inner, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeList.GetCapacity(this.inner);
    }

    public readonly Span<T> this[Range range] => this.AsSpan()[range];

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeList.GetCount(this.inner);
    }

    public readonly bool IsCreated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.inner != null;
    }

    public readonly bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.inner == null;
    }

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Count == 0;
    }

    public readonly bool IsFixedSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeList.IsFixedSize(this.inner);
    }

    public NativeList() : this(4)
    { }

    public NativeList(ReadOnlySpan<T> values, bool fixedSize = false) : this(values.Length > 0 ? values.Length : 4, fixedSize)
    {
        UnsafeList.SetCount(this.inner, values.Length);

        values.CopyTo(this);
    }

    internal readonly UnsafeList* GetUnsafeList() =>
        this.inner;

    public void Dispose()
    {
        UnsafeList.Free(this.inner);

        this.inner = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Add(T item) =>
        UnsafeList.Add(this.inner, item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() =>
        UnsafeList.GetSpan<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Clear() =>
        UnsafeList.Clear(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(Span<T> destination) =>
        this.AsSpan().CopyTo(destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly UnsafeList.Enumerator<T> GetEnumerator() =>
        UnsafeList.GetEnumerator<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Insert(int index, in T item) =>
        UnsafeList.Insert(this.inner, index, item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void RemoveAt(int index) =>
        UnsafeList.RemoveAt(this.inner, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void RemoveAt(int startIndex, int count) =>
        UnsafeList.RemoveAt(this.inner, startIndex, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start) =>
        UnsafeList.GetSpan<T>(this.inner, start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start, int length) =>
        UnsafeList.GetSpan<T>(this.inner, start, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T[] ToArray() =>
        this.AsSpan().ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly NativeArray<T> ToNativeArray() =>
        new(this);

    public override readonly string ToString() =>
        $"Count = {this.Count}";

    public static implicit operator T*(NativeList<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeList<T> value) => value.AsSpan();
}
