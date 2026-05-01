using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeArray<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeArray))]
public unsafe partial struct NativeArray<T>(int size) : IDisposable where T : unmanaged
{
    private UnsafeArray* inner = UnsafeArray.Allocate<T>(size);

    public readonly T* Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (T*)UnsafeArray.GetBuffer(this.inner);
    }

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref UnsafeArray.GetRef<T>(this.inner, index);
    }

    public readonly bool IsCreated => this.inner != null;

    public readonly bool IsEmpty => this.Length == 0;

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeArray.GetLength(this.inner);
    }

    public NativeArray(ReadOnlySpan<T> values) : this(values.Length) =>
        Copy(values, this);

    public static void Copy(NativeArray<T> src, NativeArray<T> dst) =>
        Copy(src, 0, dst, 0, src.Length);

    public static void Copy(NativeArray<T> src, NativeArray<T> dst, int length) =>
        Copy(src, 0, dst, 0, length);

    public static void Copy(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length) =>
        UnsafeArray.Copy<T>(src.inner, srcIndex, dst.inner, dstIndex, length);

    public static void Copy(NativeArray<T> src, Span<T> dst) =>
        Copy(src, 0, dst, 0, src.Length);

    public static void Copy(NativeArray<T> src, Span<T> dst, int length) =>
        Copy(src, 0, dst, 0, length);

    public static void Copy(NativeArray<T> src, int srcIndex, Span<T> dst, int dstIndex, int length)
    {
        Debug.Assert(src.IsCreated);
        Debug.Assert(src.Length >= srcIndex + length);
        Debug.Assert(dst.Length >= dstIndex + length);

        src.Slice(srcIndex, length).CopyTo(dst.Slice(dstIndex, length));
    }

    public static void Copy(ReadOnlySpan<T> src, NativeArray<T> dst) =>
        Copy(src, 0, dst, 0, src.Length);

    public static void Copy(ReadOnlySpan<T> src, NativeArray<T> dst, int length) =>
        Copy(src, 0, dst, 0, length);

    public static void Copy(ReadOnlySpan<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
    {
        Debug.Assert(src.Length >= srcIndex + length);
        Debug.Assert(dst.IsCreated);
        Debug.Assert(dst.Length >= dstIndex + length);

        src.Slice(srcIndex, length).CopyTo(dst.Slice(dstIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() =>
        UnsafeArray.GetSpan<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyFrom(ReadOnlySpan<T> array) =>
        Copy(array, 0, this, 0, array.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyFrom(NativeArray<T> array) =>
        Copy(array, this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(Span<T> array) =>
        Copy(this, array, this.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(NativeArray<T> array) =>
        Copy(this, array);

    public void Dispose()
    {
        UnsafeArray.Free(this.inner);

        this.inner = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly UnsafeArray.Enumerator<T> GetEnumerator() =>
        UnsafeArray.GetEnumerator<T>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start) =>
        UnsafeArray.GetSpan<T>(this.inner, start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start, int length) =>
        UnsafeArray.GetSpan<T>(this.inner, start, length);

    public readonly T[] ToArray() =>
        this.AsSpan().ToArray();

    public static implicit operator Span<T>(NativeArray<T> value) => value.AsSpan();
    public static implicit operator T*(NativeArray<T> value) => value.Buffer;
}
