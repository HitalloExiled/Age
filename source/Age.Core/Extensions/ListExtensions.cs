using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static class ListExtensions
{
    public static Span<T> AsSpan<T>(this List<T> source) =>
        CollectionsMarshal.AsSpan(source);
}
