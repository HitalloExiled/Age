using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int size, T defaultValue)
        {
            source.EnsureCapacity(size);

            if (size > source.Count)
            {
                var previous = source.Count;

                source.SetCount(size);

                var span = source.AsSpan();

                for (var i = previous; i < span.Length; i++)
                {
                    span[i] = defaultValue;
                }
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
