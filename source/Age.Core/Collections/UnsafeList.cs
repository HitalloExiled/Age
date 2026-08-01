using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeList
{
    private UnsafeBuffer items;
    private int count;

#if DEBUG
    private nint typeHandle;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards(UnsafeList* list)
    {
        Debug.Assert(list != null);
        Debug.Assert(list->items.Pointer != null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards<T>(UnsafeList* list)
    {
        AssertSafeGuards(list);
#if DEBUG
        Debug.Assert(typeof(T).TypeHandle.Value == list->typeHandle);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSafeGuards<T>(UnsafeList* list)
    {
#if DEBUG
        Debug.Assert(list != null);
        list->typeHandle = typeof(T).TypeHandle.Value;
#endif
    }

    private static void EnsureCapacity(UnsafeList* list)
    {
        if (list->count + 1 > list->items.Length)
        {
            if (list->items.Dynamic == 0)
            {
                throw new InvalidOperationException(ExceptionMessages.INVALID_OPERATION_COLLECTION_FULL);
            }

            SetCapacity(list, Math.Max(2, list->items.Length * 2));
        }
    }

    public static void Add<T>(UnsafeList* list, T item) where T : unmanaged
    {
        AssertSafeGuards<T>(list);

        var count = list->count;
        var items = list->items;

        EnsureCapacity(list);

        Debug.Assert(count < list->items.Length);

        *list->items.Element<T>(count) = item;

        list->count = count + 1;
    }

    public static UnsafeList* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), string.Format(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_MUST_BE_POSITIVE, nameof(capacity)));
        }

        var stride = sizeof(T);

        UnsafeList* list;

        if (fixedSize)
        {
            var alignment = Marshal.GetAlignment(stride);

            var sizeOfHeader = Marshal.RoundToAlignment(sizeof(UnsafeList), alignment);
            var sizeOfBuffer = stride * capacity;

            var ptr = NativeMemory.AllocZeroed((nuint)(sizeOfHeader + sizeOfBuffer));

            list = (UnsafeList*)ptr;

            UnsafeBuffer.InitFixed(&list->items, (byte*)ptr + sizeOfHeader, capacity, stride);
        }
        else
        {
            list = NativeMemory.AllocZeroed<UnsafeList>();

            UnsafeBuffer.InitDynamic(&list->items, capacity, stride);
        }

        list->count = 0;

        SetSafeGuards<T>(list);

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(UnsafeList* list)
    {
        AssertSafeGuards(list);

        list->count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T> =>
        IndexOf(list, item) > -1;

    public static void CopyTo<T>(UnsafeList* list, void* destination, int destinationIndex) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);

        AssertSafeGuards<T>(list);

        var numToCopy = list->count;
        if (numToCopy == 0)
        {
            return;
        }

        UnsafeBuffer.CopyTo<T>(list->items, 0, destination, destinationIndex, numToCopy);
    }

    public static void Free(UnsafeList* list)
    {
        if (list == null)
        {
            return;
        }

        if (list->items.Dynamic == 1)
        {
            UnsafeBuffer.Free(&list->items);
        }

        *list = default;

        NativeMemory.Free(list);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* GetBuffer(UnsafeList* list)
    {
        AssertSafeGuards(list);

        return list->items.Pointer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCount(UnsafeList* list)
    {
        AssertSafeGuards(list);

        return list->count;
    }

    public static int GetCapacity(UnsafeList* list)
    {
        Debug.Assert(list != null);
        Debug.Assert(list->items.Pointer != null);

        return list->items.Length;
    }

    public static void Insert<T>(UnsafeList* list, int index, T item) where T : unmanaged
    {
        AssertSafeGuards(list);

        var count = list->count;

        EnsureCapacity(list);

        if (index < count)
        {
            UnsafeBuffer.Move(list->items, index, index + 1, count - index);
        }

        *list->items.Element<T>(index) = item;

        list->count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFixedSize(UnsafeList* list)
    {
        AssertSafeGuards(list);

        return list->items.Dynamic == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>(UnsafeList* list, int index) where T : unmanaged =>
        *GetPtr<T>(list, index);

    public static Enumerator<T> GetEnumerator<T>(UnsafeList* list) where T : unmanaged
    {
        AssertSafeGuards<T>(list);

        return new Enumerator<T>(list->items, 0, list->count);
    }

    public static T* GetPtr<T>(UnsafeList* list, int index) where T : unmanaged
    {
        AssertSafeGuards<T>(list);

        if ((uint)index >= (uint)list->count)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        var items = list->items;

        return items.Element<T>(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<T>(UnsafeList* list, int index) where T : unmanaged =>
        ref *GetPtr<T>(list, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpan<T>(UnsafeList* list) where T : unmanaged =>
        GetSpan<T>(list, 0, list->count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpan<T>(UnsafeList* list, int start) where T : unmanaged =>
        GetSpan<T>(list, start, list->count - start);

    public static Span<T> GetSpan<T>(UnsafeList* list, int start, int length) where T : unmanaged
    {
        AssertSafeGuards<T>(list);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(start + length, list->count);

        return new((T*)list->items.Pointer + start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(list);

        return UnsafeBuffer.IndexOf(list->items, item, 0, list->count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(list);

        return UnsafeBuffer.LastIndexOf(list->items, item, list->count - 1, list->count);
    }

    public static bool Remove<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(list);

        var index = IndexOf(list, item);

        if (index < 0)
        {
            return false;
        }

        RemoveAt(list, index);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAt(UnsafeList* list, int index) =>
        RemoveAt(list, index, 1);

    public static void RemoveAt(UnsafeList* list, int startIndex, int count)
    {
        AssertSafeGuards(list);

        if ((uint)startIndex > (uint)count)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        var fromIndex    = startIndex + count;
        var itemToRemove = list->count - fromIndex;

        if (itemToRemove > 0)
        {
            UnsafeBuffer.Move(list->items, fromIndex, startIndex, itemToRemove);
        }

        list->count = int.Max(list->count - count, 0);
    }

    public static void RemoveAtUnordered(UnsafeList* list, int index)
    {
        AssertSafeGuards(list);

        var count = list->count;

        if ((uint)index >= (uint)count)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        list->count = --count;

        if (index < count)
        {
            UnsafeBuffer.Move(list->items, count, index, 1);
        }
    }

    public static bool RemoveUnordered<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
    {
        var index = IndexOf(list, item);

        if (index < 0)
        {
            return false;
        }

        RemoveAtUnordered(list, index);

        return true;
    }

    public static void Set<T>(UnsafeList* list, int index, T item) where T : unmanaged
    {
        AssertSafeGuards<T>(list);

        if ((uint)index >= (uint)list->count)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        *list->items.Element<T>(index) = item;
    }

    public static void SetCapacity(UnsafeList* list, int capacity)
    {
        AssertSafeGuards(list);

        Debug.Assert(capacity > 0);

        if (list->items.Dynamic == 0)
        {
            throw new InvalidOperationException(ExceptionMessages.INVALID_OPERATION_COLLECTION_FULL);
        }

        if (capacity == list->items.Length)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, list->count);

        UnsafeBuffer.ResizeDynamic(&list->items, capacity, false);
    }

    public static void SetCount(UnsafeList* list, int count)
    {
        AssertSafeGuards(list);

        if (count < 0 || count > list->items.Length)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_MUST_BE_LESS_THAN_CAPACITY);
        }

        list->count = count;
    }
}
