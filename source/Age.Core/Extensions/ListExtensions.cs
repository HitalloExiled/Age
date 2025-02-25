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
            while (size > source.Count)
            {
                source.Add(defaultValue);
            }
        }
        else
        {
            source.RemoveRange(0, source.Count - size);
        }
    }
}
