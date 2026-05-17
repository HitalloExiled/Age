using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeStack<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStack))]
public unsafe partial struct NativeStack<T>(int capacity, bool fixedSize = false) : IDisposable where T : unmanaged
{
    private UnsafeStack* inner = UnsafeStack.Allocate<T>(capacity, fixedSize);

    public readonly T* Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (T*)UnsafeStack.GetBuffer(this.inner);
    }

    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => UnsafeStack.SetCapacity(this.inner, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeStack.GetCapacity(this.inner);
    }

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeStack.GetCount(this.inner);
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

    public readonly bool IsFixedSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeStack.IsFixedSize(this.inner);
    }

    public NativeStack() : this(4)
    { }

    public NativeStack(ReadOnlySpan<T> values, bool fixedSize = false) : this(values.Length > 0 ? values.Length : 4, fixedSize)
    {
        UnsafeStack.SetCount(this.inner, values.Length);

        values.CopyTo(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly UnsafeStack* GetUnsafeStack() =>
        this.inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() =>
        UnsafeStack.GetSpan<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Clear() =>
        UnsafeStack.Clear(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(Span<T> span, int arrayIndex) =>
        this.Slice(arrayIndex).CopyTo(span);

    public void Dispose()
    {
        UnsafeStack.Free(this.inner);
        this.inner = null;
    }

    public readonly UnsafeStack.Enumerator<T> GetEnumerator() =>
        UnsafeStack.GetEnumerator<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Peek() =>
        UnsafeStack.Peek<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Pop() =>
        UnsafeStack.Pop<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Push(T item) =>
        UnsafeStack.Push(this.inner, item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start) =>
        UnsafeStack.GetSpan<T>(this.inner, start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start, int length) =>
        UnsafeStack.GetSpan<T>(this.inner, start, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T[] ToArray()
    {
        var array = this.AsSpan().ToArray();

        array.AsSpan().Reverse();

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly NativeArray<T> ToNativeArray()
    {
        var array = new NativeArray<T>(this);

        array.AsSpan().Reverse();

        return array;
    }

    public override readonly string ToString() =>
        $"Count = {this.Count}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryPeek(out T item) =>
        UnsafeStack.TryPeek(this.inner, out item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryPop(out T item) =>
        UnsafeStack.TryPop(this.inner, out item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryPush(T item) =>
        UnsafeStack.TryPush(this.inner, item);

    public static implicit operator T*(NativeStack<T> value) => value.Buffer;
    public static implicit operator Span<T>(NativeStack<T> value) => value.AsSpan();
}
