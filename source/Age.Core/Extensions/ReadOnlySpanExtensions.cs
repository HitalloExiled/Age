using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpan<U> Cast<T, U>(this ReadOnlySpan<T> span)
    where T : struct
    where U : struct =>
        MemoryMarshal.Cast<T, U>(span);
}
