using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ThirdParty.Skia.Svg;

internal static partial class Extension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> Slice(this ReadOnlySpan<char> source, in ValueMatch match) =>
        source.Slice(match.Index, match.Length);
}
