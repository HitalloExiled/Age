using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static class ListExtensions
{
    public static Span<T> AsSpan<T>(this List<T> source) =>
        CollectionsMarshal.AsSpan(source);

    public static Span<T> AsSpan<T>(this List<T> source, int start) =>
        CollectionsMarshal.AsSpan(source)[start..];

    public static Span<T> AsSpan<T>(this List<T> source, int start, int end) =>
        CollectionsMarshal.AsSpan(source)[start..end];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Resize<T>(this List<T> source, int size, T defaultValue)
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
    public static void SetCount<T>(this List<T> source, int count) =>
        CollectionsMarshal.SetCount(source, count);

}
