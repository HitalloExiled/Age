using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    private static void Resize<T>(List<T> source, int size)
    {
        source.EnsureCapacity(size);
        source.SetCount(size);
    }

    extension<T>(List<T> source)
    {
        public Span<T> AsSpan() =>
            CollectionsMarshal.AsSpan(source);

        public Span<T> AsSpan(int start) =>
            CollectionsMarshal.AsSpan(source)[start..];

        public Span<T> AsSpan(int start, int length) =>
            CollectionsMarshal.AsSpan(source).Slice(start, length);

        public Span<T> AsSpan(Range range) =>
            CollectionsMarshal.AsSpan(source)[range];

        public void Replace(Range range, ReadOnlySpan<T> values)
        {
            var (offset, length) = range.GetOffsetAndLength(source.Count);

            if (length == values.Length)
            {
                values.CopyTo(source.AsSpan(offset));
            }
            else if (length > values.Length)
            {
                values.CopyTo(source.AsSpan(offset));

                source.RemoveRange(offset + values.Length, length - values.Length);
            }
            else
            {
                var size      = source.Count + (values.Length - length);
                var remaining = source.Count - (offset + length);

                Resize(source, size);

                var from = source.AsSpan(offset + length, remaining);
                var to   = source.AsSpan(size - remaining);

                from.CopyTo(to);

                values.CopyTo(source.AsSpan(offset));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int size, T defaultValue)
        {
            if (size > source.Count)
            {
                var start = source.Count;

                Resize(source, size);

                source.AsSpan(start).Fill(defaultValue);
            }
            else
            {
                source.RemoveRange(0, source.Count - size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCount(int count) =>
            CollectionsMarshal.SetCount(source, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TimSort(Func<T, T, int>? comparer = null) =>
            source.AsSpan().TimSort(comparer);
    }
}
