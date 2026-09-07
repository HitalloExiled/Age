using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

file static class ListUnsafeAcessor<T>
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
    public static extern ref T[] GetListItems(List<T> list);
}

public static partial class Extension
{
    private static void Resize<T>(List<T> source, int size)
    {
        source.EnsureCapacity(size);
        source.SetCount(size);
    }

    extension<T>(List<T> source)
    {
        public Memory<T> AsMemory()
        {
            var items = ListUnsafeAcessor<T>.GetListItems(source);

            return new Memory<T>(items, 0, source.Count);
        }

        public Memory<U> AsMemory<U>() where U : T =>
            Unsafe.As<List<U>>(source).AsMemory();

        public Memory<T> AsMemory(int start) =>
            source.AsMemory()[start..];

        public Memory<U> AsMemory<U>(int start) where U : T =>
            source.AsMemory<T, U>()[start..];

        public Memory<T> AsMemory(int start, int length) =>
            source.AsMemory().Slice(start, length);

        public Memory<U> AsMemory<U>(int start, int length) where U : T =>
            source.AsMemory<T, U>().Slice(start, length);

        public Memory<T> AsMemory(Range range) =>
            source.AsMemory()[range];

        public Memory<U> AsMemory<U>(Range range) where U : T =>
            source.AsMemory<T, U>()[range];

        public Span<T> AsSpan() =>
            CollectionsMarshal.AsSpan(source);

        public Span<U> AsSpan<U>() where U : T =>
            CollectionsMarshal.AsSpan(Unsafe.As<List<U>>(source));

        public Span<T> AsSpan(int start) =>
            source.AsSpan()[start..];

        public Span<U> AsSpan<U>(int start) where U : T =>
            source.AsSpan<T, U>()[start..];

        public Span<T> AsSpan(int start, int length) =>
            source.AsSpan().Slice(start, length);

        public Span<U> AsSpan<U>(int start, int length) where U : T =>
            source.AsSpan<T, U>().Slice(start, length);

        public Span<T> AsSpan(Range range) =>
            source.AsSpan()[range];

        public Span<U> AsSpan<U>(Range range) where U : T =>
            source.AsSpan<T, U>()[range];

        public void ReplaceRange(Range range, ReadOnlySpan<T> values)
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
