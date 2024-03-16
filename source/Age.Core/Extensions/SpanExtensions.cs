using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static class SpanExtensions
{
    public static Span<U> Cast<T, U>(this Span<T> span)
    where T : struct
    where U : struct =>
        MemoryMarshal.Cast<T, U>(span);
}
