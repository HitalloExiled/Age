using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension<T>(ReadOnlySpan<T> span) where T : struct
    {
        public ReadOnlySpan<U> Cast<U>() where U : struct =>
            MemoryMarshal.Cast<T, U>(span);
    }
}
