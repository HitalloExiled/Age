using Age.Core.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeStack
{
    private const int DEFAULT_CAPACITY = 8;

    private UnsafeBuffer items;
#if DEBUG
    private IntPtr typeHandle;
#endif
    private int count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards(UnsafeStack* stack)
    {
        Debug.Assert(stack != null);
        Debug.Assert(stack->items.Pointer != null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards<T>(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);
#if DEBUG
        Debug.Assert(typeof(T).TypeHandle.Value == stack->typeHandle);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSafeGuards<T>(UnsafeStack* stack)
    {
#if DEBUG
        Debug.Assert(stack != null);
        stack->typeHandle = typeof(T).TypeHandle.Value;
#endif
    }

    private static void ResizeAndPush<T>(UnsafeStack* stack, T item) where T : unmanaged
    {
        Expand(stack);

        *stack->items.Element<T>(stack->count) = item;
        stack->count++;
    }

    private static void Expand(UnsafeStack* stack)
    {
        var length = stack->items.Length == 0 ? DEFAULT_CAPACITY : stack->items.Length * 2;

        UnsafeBuffer.ResizeDynamic(&stack->items, length, false);
    }

    public static UnsafeStack* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), string.Format(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_MUST_BE_POSITIVE, nameof(capacity)));
        }

        var stride = sizeof(T);

        UnsafeStack* stack;

        if (fixedSize)
        {
            var alignment = Marshal.GetAlignment(stride);

            var sizeOfStack = Marshal.RoundToAlignment(sizeof(UnsafeStack), alignment);
            var sizeOfArray = stride * capacity;

            var ptr = NativeMemory.AlignedAllocZeroed((nuint)(sizeOfStack + sizeOfArray), (nuint)alignment);

            stack = (UnsafeStack*)ptr;

            UnsafeBuffer.InitFixed(&stack->items, (byte*)ptr + sizeOfStack, capacity, stride);
        }
        else
        {
            stack = NativeMemory.AllocZeroed<UnsafeStack>();

            UnsafeBuffer.InitDynamic(&stack->items, capacity, stride);
        }

        stack->count = 0;

        SetSafeGuards<T>(stack);

        return stack;
    }

    public static void Clear(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);

        stack->count = 0;
    }

    public static bool Contains<T>(UnsafeStack* stack, T item) where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(stack);

        var count = stack->count;

        return count != 0 && UnsafeBuffer.LastIndexOf(stack->items, item, count - 1, count) != -1;
    }

    public static void CopyTo<T>(UnsafeStack* stack, void* destination, int destinationIndex) where T : unmanaged
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);

        AssertSafeGuards<T>(stack);

        var numToCopy = stack->count;
        if (numToCopy == 0)
        {
            return;
        }

        var srcIndex = 0;
        var stride = stack->items.Stride;
        var dstIndex = destinationIndex + numToCopy;

        while (srcIndex < numToCopy)
        {
            *(T*)((byte*)destination + (--dstIndex * stride)) = *stack->items.Element<T>(srcIndex++);
        }
    }

    public static void Free(UnsafeStack* stack)
    {
        if (stack == null)
        {
            return;
        }

        if (stack->items.Dynamic == 1)
        {
            UnsafeBuffer.Free(&stack->items);
        }

        *stack = default;

        NativeMemory.Free(stack);
    }

    public static int GetCapacity(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);

        return stack->items.Length;
    }

    public static int GetCount(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);

        return stack->count;
    }

    public static void* GetBuffer(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);

        return stack->items.Pointer;
    }

    public static Span<T> GetSpan<T>(UnsafeStack* stack) where T : unmanaged =>
        GetSpan<T>(stack, 0, stack->count);

    public static Span<T> GetSpan<T>(UnsafeStack* stack, int start) where T : unmanaged =>
        GetSpan<T>(stack, start, stack->count - start);

    public static Span<T> GetSpan<T>(UnsafeStack* stack, int start, int length) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(start + length, stack->count);

        return new((T*)stack->items.Pointer + start, length);
    }

    public static Enumerator<T> GetEnumerator<T>(UnsafeStack* stack) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        return new Enumerator<T>(stack->items, stack->count);
    }

    public static bool IsFixedSize(UnsafeStack* stack)
    {
        AssertSafeGuards(stack);

        return stack->items.Dynamic == 0;
    }

    public static T Peek<T>(UnsafeStack* stack) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var count = stack->count - 1;

        return (uint)count >= (uint)stack->items.Length
            ? throw new InvalidOperationException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX)
            : *stack->items.Element<T>(count);
    }

    public static T Pop<T>(UnsafeStack* stack) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var count = stack->count - 1;

        if ((uint)count >= (uint)stack->items.Length)
        {
            throw new InvalidOperationException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
        }

        stack->count = count;

        return *stack->items.Element<T>(count);
    }

    public static void Push<T>(UnsafeStack* stack, T item) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var items = stack->items;
        var count = stack->count;

        if ((uint)count < (uint)items.Length)
        {
            *items.Element<T>(count) = item;
            stack->count = count + 1;
        }
        else
        {
            if (items.Dynamic == 1)
            {
                ResizeAndPush(stack, item);
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_INDEX);
            }
        }
    }

    public static void SetCapacity(UnsafeStack* stack, int capacity)
    {
        AssertSafeGuards(stack);

        Debug.Assert(capacity > 0);

        if (stack->items.Dynamic == 0)
        {
            throw new InvalidOperationException(ExceptionMessages.INVALID_OPERATION_COLLECTION_FULL);
        }

        if (capacity == stack->items.Length)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, stack->count);

        UnsafeBuffer.ResizeDynamic(&stack->items, capacity, false);
    }

    public static void SetCount(UnsafeStack* stack, int count)
    {
        AssertSafeGuards(stack);

        if (count < 0 || count > stack->items.Length)
        {
            throw new IndexOutOfRangeException(ExceptionMessages.ARGUMENT_OUT_OF_RANGE_MUST_BE_LESS_THAN_CAPACITY);
        }

        stack->count = count;
    }

    public static bool TryPeek<T>(UnsafeStack* stack, out T item) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var count = stack->count - 1;

        if ((uint)count >= (uint)stack->items.Length)
        {
            item = default;
            return false;
        }

        item = *stack->items.Element<T>(count);

        return true;
    }

    public static bool TryPop<T>(UnsafeStack* stack, out T item) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var count = stack->count - 1;

        if ((uint)count >= (uint)stack->items.Length)
        {
            item = default;
            return false;
        }

        stack->count = count;
        item = *stack->items.Element<T>(count);

        return true;
    }

    public static bool TryPush<T>(UnsafeStack* stack, T item) where T : unmanaged
    {
        AssertSafeGuards<T>(stack);

        var items = stack->items;
        var count = stack->count;

        if ((uint)count < (uint)items.Length)
        {
            *items.Element<T>(count) = item;

            stack->count = count + 1;

            return true;
        }
        else
        {
            if (items.Dynamic == 1)
            {
                ResizeAndPush(stack, item);

                return true;
            }
            return false;
        }
    }
}
