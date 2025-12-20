using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;
public static partial class Extension
{
    private const int RUN = 32;

    private static void InsertionSort<T>(Func<T, T, int> comparer, Span<T> span, int leftIndex, int rightIndex)
    {
        for (var currentIndex = leftIndex + 1; currentIndex <= rightIndex; currentIndex++)
        {
            var currentValue = span[currentIndex];
            var sortedIndex = currentIndex - 1;

            while (sortedIndex >= leftIndex && comparer.Invoke(span[sortedIndex], currentValue) == 1)
            {
                span[sortedIndex + 1] = span[sortedIndex];
                sortedIndex--;
            }

            span[sortedIndex + 1] = currentValue;
        }
    }

    private static void Merge<T>(Func<T, T, int> comparer, Span<T> span, int leftIndex, int middleIndex, int rightIndex)
    {
        var leftLength  = middleIndex - leftIndex + 1;
        var rightLength = rightIndex - middleIndex;

        var rentedLeft  = ArrayPool<T>.Shared.Rent(leftLength);
        var rentedRight = ArrayPool<T>.Shared.Rent(rightLength);

        var left  = rentedLeft.AsSpan(0, leftLength);
        var right = rentedRight.AsSpan(0, rightLength);

        span.Slice(leftIndex, leftLength).CopyTo(left);
        span.Slice(middleIndex + 1, rightLength).CopyTo(right);

        var leftPointer  = 0;
        var rightPointer = 0;
        var mergedIndex  = leftIndex;

        while (leftPointer < left.Length && rightPointer < right.Length)
        {
            span[mergedIndex++] = comparer.Invoke(left[leftPointer], right[rightPointer]) <= 0 ? left[leftPointer++] : right[rightPointer++];
        }

        if (leftPointer < left.Length)
        {
            left[leftPointer..].CopyTo(span[mergedIndex..]);
            mergedIndex += left.Length - leftPointer;
        }

        if (rightPointer < right.Length)
        {
            right[rightPointer..].CopyTo(span[mergedIndex..]);
        }

        ArrayPool<T>.Shared.Return(rentedLeft, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        ArrayPool<T>.Shared.Return(rentedRight, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    }

    extension<T>(Span<T> span) where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<U> Cast<U>() where U : struct =>
            MemoryMarshal.Cast<T, U>(span);
    }

    extension<T>(Span<T> span)
    {
        public DiffResult<T> Diff(ReadOnlySpan<T> other)
        {
            var addedBuffer   = ArrayPool<T>.Shared.Rent(other.Length);
            var removedBuffer = ArrayPool<T>.Shared.Rent(span.Length);

            var updatedSet = new HashSet<T>(other.Length);

            foreach (var item in other)
            {
                updatedSet.Add(item);
            }

            var removedCount = 0;

            foreach (var item in span)
            {
                if (!updatedSet.Remove(item))
                {
                    removedBuffer[removedCount++] = item;
                }
            }

            updatedSet.CopyTo(addedBuffer);

            return new DiffResult<T>(addedBuffer, updatedSet.Count, removedBuffer, removedCount);
        }

        public void TimSort(Func<T, T, int>? comparer = null)
        {
            comparer ??= Comparer<T>.Default.Compare;

            for (var i = 0; i < span.Length; i += RUN)
            {
                InsertionSort(comparer, span, i, int.Min(i + RUN - 1, span.Length - 1));
            }

            for (var subarraySize = RUN; subarraySize < span.Length; subarraySize *= 2)
            {
                for (var leftStart = 0; leftStart < span.Length; leftStart += 2 * subarraySize)
                {
                    var middleIndex = int.Min(leftStart + subarraySize - 1, span.Length - 1);
                    var rightEnd    = int.Min(leftStart + (2 * subarraySize) - 1, span.Length - 1);

                    if (middleIndex < rightEnd)
                    {
                        Merge(comparer, span, leftStart, middleIndex, rightEnd);
                    }
                }
            }
        }
    }

    extension<T>(Span<T> span) where T : IEquatable<T>
    {
        public Memory<T> Intersect(Span<T> other)
        {
            if (span.IsEmpty || other.IsEmpty)
            {
                return Memory<T>.Empty;
            }

            Span<T> lookupSource, iterateSource;

            if (span.Length < other.Length)
            {
                iterateSource = other;
                lookupSource  = span;
            }
            else
            {
                iterateSource = span;
                lookupSource  = other;
            }

            var set = new HashSet<T>(lookupSource.Length);

            var destination = new T[int.Max(span.Length, other.Length)];

            foreach (var item in lookupSource)
            {
                set.Add(item);
            }

            var count = 0;

            foreach (var item in iterateSource)
            {
                if (set.Remove(item))
                {
                    destination[count++] = item;
                }
            }

            return destination.AsMemory(0, count);
        }
    }
}
