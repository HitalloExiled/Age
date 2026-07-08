using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeArray
{
    private void* buffer;

    private int length;

#if DEBUG
    private nint typeHandle;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards<T>(UnsafeArray* array)
    {
#if DEBUG
        Debug.Assert(array != null);
        Debug.Assert(typeof(T).TypeHandle.Value == array->typeHandle);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSafeGuards<T>(UnsafeArray* array)
    {
#if DEBUG
        Debug.Assert(array != null);
        array->typeHandle = typeof(T).TypeHandle.Value;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void AssertSafeGuards<T>()
    {
#if DEBUG
        Debug.Assert(typeof(T).TypeHandle.Value == this.typeHandle);
#endif
    }

    internal static void* GetBuffer(UnsafeArray* array) =>
        array->buffer;

    public static UnsafeArray* Allocate<T>(int size) where T : unmanaged
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "size must be non-negative.");
        }

        var alignment       = Marshal.GetAlignment(sizeof(T));
        var arrayStructSize = Marshal.RoundToAlignment(sizeof(UnsafeArray), alignment);
        var arrayMemorySize = size * sizeof(T);
        var ptr             = NativeMemory.AlignedAllocZeroed((nuint)(arrayStructSize + arrayMemorySize), (nuint)alignment);
        var array           = (UnsafeArray*)ptr;

        array->buffer = ((byte*)ptr) + arrayStructSize;
        array->length = size;

        SetSafeGuards<T>(array);

        return array;
    }

    public static void Clear<T>(UnsafeArray* array) where T : unmanaged =>
        NativeMemory.Clear(array->buffer, (nuint)(array->length * sizeof(T)));

    public static bool Contains<T>(UnsafeArray* array, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(array);

        return IndexOf(array, item) > -1;
    }

    public static void Copy<T>(UnsafeArray* source, int sourceIndex, UnsafeArray* destination, int destinationIndex, int count) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        AssertSafeGuards<T>(source);
        AssertSafeGuards<T>(destination);

        if (GetLength(source) - sourceIndex < count)
        {
            throw new ArgumentException(ExceptionMessages.ARG_ARRAY_PLUS_OFF_TOO_SMALL);
        }

        if (GetLength(destination) - destinationIndex < count)
        {
            throw new ArgumentException(ExceptionMessages.ARG_ARRAY_PLUS_OFF_TOO_SMALL);
        }

        NativeMemory.Copy((T*)destination->buffer + destinationIndex, (T*)source->buffer + sourceIndex, (nuint)(count * sizeof(T)));
    }

    public static void Free(UnsafeArray* array)
    {
        if (array == null)
        {
            return;
        }

        *array = default;

        NativeMemory.AlignedFree(array);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>(UnsafeArray* array, int index) where T : unmanaged =>
        *GetPtr<T>(array, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>(UnsafeArray* array, long index) where T : unmanaged =>
        *GetPtr<T>(array, index);

    public static Enumerator<T> GetEnumerator<T>(UnsafeArray* array) where T : unmanaged
    {
        AssertSafeGuards<T>(array);

        return new Enumerator<T>(array);
    }

    public static int GetLength(UnsafeArray* array)
    {
        Debug.Assert(array != null);

        return array->length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<T>(UnsafeArray* array, int index) where T : unmanaged =>
        ref *GetPtr<T>(array, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<T>(UnsafeArray* array, long index) where T : unmanaged =>
        ref *GetPtr<T>(array, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPtr<T>(UnsafeArray* array, int index) where T : unmanaged => GetPtr<T>(array, (long)index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPtr<T>(UnsafeArray* array, long index) where T : unmanaged
    {
        AssertSafeGuards<T>(array);

        return (uint)index >= (uint)array->length
            ? throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX)
            : (T*)array->buffer + index;
    }

    public static Span<T> GetSpan<T>(UnsafeArray* array)
    where T : unmanaged =>
        GetSpan<T>(array, 0, array->length);

    public static Span<T> GetSpan<T>(UnsafeArray* array, int start)
    where T : unmanaged =>
        GetSpan<T>(array, 0, array->length - start);

    public static Span<T> GetSpan<T>(UnsafeArray* array, int start, int length)
    where T : unmanaged
    {
        AssertSafeGuards<T>(array);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(start + length, array->length);

        return new(((T*)array->buffer) + start, length);
    }

    public static int IndexOf<T>(UnsafeArray* array, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(array);

        for (var i = 0; i < GetLength(array); ++i)
        {
            if (Get<T>(array, i).Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(UnsafeArray* array, int index, T value) where T : unmanaged =>
        Set(array, (long)index, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(UnsafeArray* array, long index, T value) where T : unmanaged
    {
        AssertSafeGuards<T>(array);

        if ((uint)index >= (uint)array->length)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        *((T*)array->buffer + index) = value;
    }

    public static int LastIndexOf<T>(UnsafeArray* array, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(array);

        for (var i = GetLength(array) - 1; i >= 0; --i)
        {
            if (Get<T>(array, i).Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public readonly void CopyFrom<T>(void* source, int sourceIndex, int count) where T : unmanaged
    {
        this.AssertSafeGuards<T>();

        ArgumentNullException.ThrowIfNull(source);

        if ((uint)sourceIndex + (uint)count > this.length)
        {
            throw new ArgumentException(ExceptionMessages.ARG_ARRAY_PLUS_OFF_TOO_SMALL);
        }

        NativeMemory.Copy(this.buffer, (T*)source + sourceIndex, (nuint)(count * sizeof(T)));
    }

    public readonly void CopyTo<T>(void* destination, int destinationIndex) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfLessThan(destinationIndex, 0, nameof(destinationIndex));

        this.AssertSafeGuards<T>();

        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        NativeMemory.Copy((T*)destination + destinationIndex, this.buffer, (nuint)(this.length * sizeof(T)));
    }
}
