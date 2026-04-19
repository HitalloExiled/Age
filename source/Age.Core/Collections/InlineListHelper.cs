using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

internal static class InlineListHelper<T>
{
    public static void Add(Span<T> buffer, T item, int capacity, ref int count)
    {
        InlineListException.ThrowsIfExceeds(count + 1, capacity);

        buffer[count++] = item;
    }

    public static void Clear(Span<T> buffer, ref int count)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            buffer.Clear();
        }

        count = 0;
    }

    public static void CopyTo(Span<T> left, Span<T> right, ref int rightCount)
    {
        left.CopyTo(right);

        rightCount = left.Length;
    }

    public static void Remove(Span<T> buffer, T item, ref int count) =>
        RemoveAt(buffer, buffer.IndexOf(item), 1, ref count);

    public static void RemoveAt(Span<T> buffer, int index, ref int count) =>
        RemoveAt(buffer, index, 1, ref count);

    public static void RemoveAt(Span<T> buffer, int startIndex, int itemsCount, ref int count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, count);
        ArgumentOutOfRangeException.ThrowIfLessThan(itemsCount - startIndex, 0);

        var endIndex = startIndex + itemsCount;
        var length   = count - endIndex;

        if (length > 0)
        {
            var source      = buffer.Slice(endIndex, length);
            var destination = buffer.Slice(startIndex, length);

            source.CopyTo(destination);
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            buffer[(startIndex + length)..].Clear();
        }

        count = int.Max(count - itemsCount, 0);
    }
}
